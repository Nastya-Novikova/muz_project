#!/bin/bash
set +e

LOG_FILE="test_full_log.txt"
BASE_URL="http://localhost:7000"
VERIFICATION_CODE="111111"
TMP_DIR=$(mktemp -d)

# Проверка наличия jq
JQ=$(command -v jq 2>/dev/null)
if [ -z "$JQ" ]; then
    for dir in "/c/Program Files/Git/usr/bin" "/usr/bin" "/bin"; do
        if [ -f "$dir/jq.exe" ]; then JQ="$dir/jq.exe"; break; fi
    done
fi
if [ -z "$JQ" ]; then
    echo "ОШИБКА: jq не найден. Установите jq"
    exit 1
fi

> "$LOG_FILE"

log() {
  local msg="[$(date +'%Y-%m-%d %H:%M:%S')] $*"
  echo "$msg" | tee -a "$LOG_FILE" >&2
}

log "=== Запуск расширенного теста API с реальной почтой ==="
log "Временная папка: $TMP_DIR"

# Пересборка и запуск
log "Пересборка контейнеров..."
docker-compose -f docker-compose.backend.yml down -v > /dev/null 2>&1
docker-compose -f docker-compose.backend.yml up --build -d

log "Ожидание доступности API..."
for i in $(seq 1 60); do
    code=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL/swagger/index.html")
    if [ "$code" = "200" ]; then
        log "API доступен"
        break
    fi
    if [ "$i" -eq 60 ]; then
        log "ОШИБКА: API не запустился"
        exit 1
    fi
    sleep 2
done

# Функция логирования
dump_response() {
  local file=$1
  local label=$2
  local http_code=$(tail -1 "$file")
  local body=$(sed '$d' "$file")
  log "$label (HTTP $http_code): $body"
}

# Регистрация пользователя с профилем
register_user() {
  local EMAIL=$1
  local USER_NAME=$2
  local PROFILE_EXTRA=$3

  log "Регистрация $EMAIL ..."

  curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/auth/code" \
    -H "Content-Type: application/json" \
    -d "{\"email\":\"$EMAIL\"}" > "$TMP_DIR/code_$EMAIL.txt"
  dump_response "$TMP_DIR/code_$EMAIL.txt" "  Код подтверждения"

  curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/auth/session" \
    -H "Content-Type: application/json" \
    -d "{\"email\":\"$EMAIL\",\"code\":\"$VERIFICATION_CODE\"}" > "$TMP_DIR/login_$EMAIL.txt"
  dump_response "$TMP_DIR/login_$EMAIL.txt" "  Вход"

  local auth_body=$(sed '$d' "$TMP_DIR/login_$EMAIL.txt")
  local TOKEN=$("$JQ" -r '.token' <<< "$auth_body")
  local USER_ID=$("$JQ" -r '.user.id' <<< "$auth_body")

  if [ "$TOKEN" = "null" ] || [ -z "$TOKEN" ]; then
    log "ОШИБКА: не удалось извлечь токен"
    exit 1
  fi

  local profile_json=$("$JQ" -n --arg name "$USER_NAME" \
    --argjson extra "$PROFILE_EXTRA" '{
    fullName: $name,
    profileType: "Individual",
    cityId: 1,
    experience: 0,
    lookingFor: "NotLooking"
  } + $extra')
  curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/profiles" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "$profile_json" > "$TMP_DIR/profile_$EMAIL.txt"
  dump_response "$TMP_DIR/profile_$EMAIL.txt" "  Создание профиля"

  local profile_body=$(sed '$d' "$TMP_DIR/profile_$EMAIL.txt")
  local PROFILE_ID=$("$JQ" -r '.id' <<< "$profile_body")

  if [ "$PROFILE_ID" = "null" ] || [ -z "$PROFILE_ID" ]; then
    log "ОШИБКА: не удалось создать профиль"
    exit 1
  fi

  echo "$TOKEN $USER_ID $PROFILE_ID"
}

# ---------- Справочные данные ----------
log "=== Справочные данные ==="
curl -s -w "\n%{http_code}" "$BASE_URL/api/metadata/cities" > "$TMP_DIR/meta_cities.txt"
curl -s -w "\n%{http_code}" "$BASE_URL/api/metadata/regions" > "$TMP_DIR/meta_regions.txt"
curl -s -w "\n%{http_code}" "$BASE_URL/api/metadata/genres" > "$TMP_DIR/meta_genres.txt"
curl -s -w "\n%{http_code}" "$BASE_URL/api/metadata/specialties" > "$TMP_DIR/meta_specialties.txt"
curl -s -w "\n%{http_code}" "$BASE_URL/api/metadata/goals" > "$TMP_DIR/meta_goals.txt"

# ---------- Пользователи ----------
log "=== Регистрация пользователей ==="

# User1 (жанры, специальность, опыт)
USER1_DATA=$(register_user "user1@test.com" "User One" '{"genreIds":[1],"specialtyIds":[1],"experience":5,"lookingFor":"LookingForMusician"}')
read TOKEN1 USER1_ID PROFILE1_ID <<< "$USER1_DATA"
log "User1: profileId=$PROFILE1_ID"

# User2 (другие параметры)
USER2_DATA=$(register_user "user2@test.com" "User Two" '{"genreIds":[1,2],"specialtyIds":[2],"cityId":2,"experience":10,"lookingFor":"LookingForBand"}')
read TOKEN2 USER2_ID PROFILE2_ID <<< "$USER2_DATA"
log "User2: profileId=$PROFILE2_ID"

# User3 (реальная почта, для проверки уведомлений)
USER3_DATA=$(register_user "chmo228098@gmail.com" "User Three" '{}')
read TOKEN3 USER3_ID PROFILE3_ID <<< "$USER3_DATA"
log "User3: profileId=$PROFILE3_ID"

# ---------- Валидация ----------
log "=== Валидация ==="
curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/events" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN1" \
  -d '{"title":"","regionId":1,"cityId":1,"address":"Test","startDateTime":"2027-01-01T00:00:00Z","maxParticipants":10}' > "$TMP_DIR/val_empty_title.txt"
dump_response "$TMP_DIR/val_empty_title.txt" "Создание с пустым названием (должен 400)"

curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/auth/session" \
  -H "Content-Type: application/json" \
  -d '{"email":"nonexist@test.com","code":"000000"}' > "$TMP_DIR/val_bad_code.txt"
dump_response "$TMP_DIR/val_bad_code.txt" "Неверный код (должен 400)"

curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/profiles" \
  -H "Content-Type: application/json" \
  -d '{"fullName":"NoAuth","profileType":"Individual","cityId":1,"experience":0,"lookingFor":"NotLooking"}' > "$TMP_DIR/val_noauth_profile.txt"
dump_response "$TMP_DIR/val_noauth_profile.txt" "Создание профиля без токена (должен 401)"

# ---------- Фильтрация профилей ----------
log "=== Фильтрация профилей ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/profiles?genreIds=1&page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/filter_genre.txt"
dump_response "$TMP_DIR/filter_genre.txt" "Профили с жанром 1"

curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/profiles?cityId=2&page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/filter_city.txt"
dump_response "$TMP_DIR/filter_city.txt" "Профили в городе 2"

curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/profiles?experienceMin=5&page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/filter_exp.txt"
dump_response "$TMP_DIR/filter_exp.txt" "Профили с опытом >=5"

curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/profiles?lookingFor=LookingForBand&page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/filter_looking.txt"
dump_response "$TMP_DIR/filter_looking.txt" "Профили ищущие группу"

# ---------- Мероприятия и фильтрация ----------
log "=== Мероприятия и фильтрация ==="
START_DATE=$(date -u -d "+7 days" +"%Y-%m-%dT%H:%M:%SZ")
EVENT1_JSON=$("$JQ" -n --arg start "$START_DATE" '{
  title: "Jazz Night",
  description: "Jazz event",
  regionId: 1,
  cityId: 1,
  address: "Moscow",
  startDateTime: $start,
  maxParticipants: 5
}')
IDEMPOTENCY_KEY=$(date +%s%N)
curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/events" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN1" \
  -H "Idempotency-Key: $IDEMPOTENCY_KEY" \
  -d "$EVENT1_JSON" > "$TMP_DIR/event1.txt"
EVENT1_ID=$("$JQ" -r '.id' <<< "$(sed '$d' "$TMP_DIR/event1.txt")")
dump_response "$TMP_DIR/event1.txt" "Создание мероприятия 1"

START_DATE2=$(date -u -d "+14 days" +"%Y-%m-%dT%H:%M:%SZ")
EVENT2_JSON=$("$JQ" -n --arg start "$START_DATE2" '{
  title: "Rock Fest",
  description: "Rock event",
  regionId: 2,
  cityId: 2,
  address: "SPb",
  startDateTime: $start,
  maxParticipants: 0
}')
IDEMPOTENCY_KEY=$(date +%s%N)
curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/events" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN1" \
  -H "Idempotency-Key: $IDEMPOTENCY_KEY" \
  -d "$EVENT2_JSON" > "$TMP_DIR/event2.txt"
EVENT2_ID=$("$JQ" -r '.id' <<< "$(sed '$d' "$TMP_DIR/event2.txt")")
dump_response "$TMP_DIR/event2.txt" "Создание мероприятия 2"

# Фильтрация
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/events?cityId=2" \
  -H "Authorization: Bearer $TOKEN2" > "$TMP_DIR/events_city2.txt"
dump_response "$TMP_DIR/events_city2.txt" "Мероприятия в городе 2"

curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/events?query=Rock" \
  -H "Authorization: Bearer $TOKEN2" > "$TMP_DIR/events_query.txt"
dump_response "$TMP_DIR/events_query.txt" "Поиск Rock"

# ---------- Регистрация User3 на мероприятие и уведомления ----------
log "=== Регистрация User3 на мероприятие и проверка уведомлений ==="
curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/events/$EVENT1_ID/registration" \
  -H "Authorization: Bearer $TOKEN3" > "$TMP_DIR/reg_user3.txt"
dump_response "$TMP_DIR/reg_user3.txt" "Регистрация User3 на Event1"

# Проверяем уведомления User3 (должно быть EventRegistration)
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/notifications?page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN3" > "$TMP_DIR/notif_user3_after_reg.txt"
dump_response "$TMP_DIR/notif_user3_after_reg.txt" "Уведомления User3 после регистрации"

# ---------- Отправка предложения User3 ----------
log "=== Предложение сотрудничества User2 -> User3 ==="
curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/suggestions" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN2" \
  -d "{\"toProfileId\":\"$PROFILE3_ID\",\"message\":\"Hello, let's collaborate\"}" > "$TMP_DIR/sugg_to_user3.txt"
dump_response "$TMP_DIR/sugg_to_user3.txt" "Отправка предложения User3"

log "Ожидание обработки Outbox..."
sleep 10

# Проверяем уведомления User3 (CollaborationReceived)
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/notifications?page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN3" > "$TMP_DIR/notif_user3_after_sugg.txt"
dump_response "$TMP_DIR/notif_user3_after_sugg.txt" "Уведомления User3 после предложения"

# ---------- Флаги и CurrentParticipants ----------
log "=== Проверка флагов ==="
# isFavorite (User2 добавил User1 в избранное)
curl -s -w "\n%{http_code}" -X PUT "$BASE_URL/api/$PROFILE1_ID/favorite" \
  -H "Authorization: Bearer $TOKEN2" > "$TMP_DIR/fav_add.txt"
dump_response "$TMP_DIR/fav_add.txt" "Добавление в избранное User2->User1"

curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/profiles/$PROFILE1_ID" \
  -H "Authorization: Bearer $TOKEN2" > "$TMP_DIR/profile1_fav.txt"
dump_response "$TMP_DIR/profile1_fav.txt" "Профиль User1 от User2 (isFavorite=true)"

# isCollaborated (User2 предложил User3)
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/profiles/$PROFILE3_ID" \
  -H "Authorization: Bearer $TOKEN2" > "$TMP_DIR/profile3_collab.txt"
dump_response "$TMP_DIR/profile3_collab.txt" "Профиль User3 от User2 (isCollaborated=true)"

# CurrentParticipants и isRegistered
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/events/$EVENT1_ID" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/event1_detailed.txt"
dump_response "$TMP_DIR/event1_detailed.txt" "Детали Event1 (Creator=User1)"

# ---------- Настройки уведомлений (проверка изменения) ----------
log "=== Настройки уведомлений User3 ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/notifications/settings" \
  -H "Authorization: Bearer $TOKEN3" > "$TMP_DIR/notif_settings_initial.txt"
dump_response "$TMP_DIR/notif_settings_initial.txt" "Начальные настройки User3"

curl -s -w "\n%{http_code}" -X PATCH "$BASE_URL/api/notifications/settings" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN3" \
  -d '{"notifyByEmail":false,"notifyByVk":true}' > "$TMP_DIR/notif_settings_update.txt"
dump_response "$TMP_DIR/notif_settings_update.txt" "Изменение настроек"

curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/notifications/settings" \
  -H "Authorization: Bearer $TOKEN3" > "$TMP_DIR/notif_settings_after.txt"
dump_response "$TMP_DIR/notif_settings_after.txt" "Настройки после изменения"

# ---------- Удаление профиля User3 ----------
log "=== Удаление профиля User3 ==="
curl -s -w "\n%{http_code}" -X DELETE "$BASE_URL/api/profiles/me" \
  -H "Authorization: Bearer $TOKEN3" > "$TMP_DIR/del_profile.txt"
dump_response "$TMP_DIR/del_profile.txt" "Удаление профиля"

curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/profiles/$PROFILE3_ID" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/del_check.txt"
dump_response "$TMP_DIR/del_check.txt" "Проверка удаления (должен 404)"

log "=== Расширенный тест завершён ==="
echo "Лог сохранён в $LOG_FILE"