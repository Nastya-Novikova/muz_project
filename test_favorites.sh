#!/bin/bash
set +e

LOG_FILE="test_favorites_log.txt"
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
    echo "ОШИБКА: jq не найден. Установите jq:"
    echo "  Git Bash: скопируйте jq.exe из https://stedolan.github.io/jq/download/ в C:\\Program Files\\Git\\usr\\bin"
    echo "  или выполните: curl -L -o /usr/bin/jq.exe https://github.com/stedolan/jq/releases/download/jq-1.6/jq-win64.exe"
    exit 1
fi

> "$LOG_FILE"

log() {
  local msg="[$(date +'%Y-%m-%d %H:%M:%S')] $*"
  echo "$msg" | tee -a "$LOG_FILE" >&2
}

log "=== Запуск полного теста API (без медиа) ==="
log "Временная папка: $TMP_DIR"

# Пересборка и запуск
log "Пересборка контейнеров (может занять пару минут)..."
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
        log "ОШИБКА: API не запустился за 2 минуты"
        exit 1
    fi
    sleep 2
done

# Функция логирования ответов
dump_response() {
  local file=$1
  local label=$2
  local http_code=$(tail -1 "$file")
  local body=$(sed '$d' "$file")
  log "$label (HTTP $http_code): $body"
}

# Регистрация пользователя (возвращает строку с token, userId, profileId)
register_user() {
  local EMAIL=$1
  local USER_NAME=$2

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

  log "  Токен: ${TOKEN:0:20}... userId=$USER_ID"

  if [ "$TOKEN" = "null" ] || [ -z "$TOKEN" ]; then
    log "ОШИБКА: не удалось извлечь токен"
    exit 1
  fi

  local profile_json=$("$JQ" -n --arg name "$USER_NAME" '{
    fullName: $name,
    profileType: "Individual",
    cityId: 1,
    experience: 0,
    lookingFor: "NotLooking"
  }')
  curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/profiles" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "$profile_json" > "$TMP_DIR/profile_$EMAIL.txt"
  dump_response "$TMP_DIR/profile_$EMAIL.txt" "  Создание профиля"

  local profile_body=$(sed '$d' "$TMP_DIR/profile_$EMAIL.txt")
  local PROFILE_ID=$("$JQ" -r '.id' <<< "$profile_body")
  log "  profileId=$PROFILE_ID"

  if [ "$PROFILE_ID" = "null" ] || [ -z "$PROFILE_ID" ]; then
    log "ОШИБКА: не удалось создать профиль"
    exit 1
  fi

  echo "$TOKEN $USER_ID $PROFILE_ID"
}

# ---------- Справочные данные ----------
log "=== Проверка справочных данных ==="
curl -s -w "\n%{http_code}" "$BASE_URL/api/metadata/cities" > "$TMP_DIR/meta_cities.txt"
dump_response "$TMP_DIR/meta_cities.txt" "Города"
curl -s -w "\n%{http_code}" "$BASE_URL/api/metadata/regions" > "$TMP_DIR/meta_regions.txt"
dump_response "$TMP_DIR/meta_regions.txt" "Регионы"
curl -s -w "\n%{http_code}" "$BASE_URL/api/metadata/genres" > "$TMP_DIR/meta_genres.txt"
dump_response "$TMP_DIR/meta_genres.txt" "Жанры"
curl -s -w "\n%{http_code}" "$BASE_URL/api/metadata/specialties" > "$TMP_DIR/meta_specialties.txt"
dump_response "$TMP_DIR/meta_specialties.txt" "Специальности"
curl -s -w "\n%{http_code}" "$BASE_URL/api/metadata/goals" > "$TMP_DIR/meta_goals.txt"
dump_response "$TMP_DIR/meta_goals.txt" "Цели сотрудничества"

# ---------- Пользователи ----------
log "=== Регистрация первого пользователя ==="
USER1_DATA=$(register_user "user1@test.com" "User One")
read TOKEN1 USER1_ID PROFILE1_ID <<< "$USER1_DATA"
log "Первый пользователь: userId=$USER1_ID, profileId=$PROFILE1_ID"

log "=== Регистрация второго пользователя ==="
USER2_DATA=$(register_user "user2@test.com" "User Two")
read TOKEN2 USER2_ID PROFILE2_ID <<< "$USER2_DATA"
log "Второй пользователь: userId=$USER2_ID, profileId=$PROFILE2_ID"

log "=== Регистрация третьего пользователя (для теста удаления) ==="
USER3_DATA=$(register_user "user3@test.com" "User Three")
read TOKEN3 USER3_ID PROFILE3_ID <<< "$USER3_DATA"
log "Третий пользователь: userId=$USER3_ID, profileId=$PROFILE3_ID"

# ---------- Профили ----------
log "=== Поиск профилей ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/profiles?query=User&page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN2" > "$TMP_DIR/search_profiles.txt"
dump_response "$TMP_DIR/search_profiles.txt" "Поиск профилей"

log "=== Получение своего профиля ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/profiles/me" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/my_profile.txt"
dump_response "$TMP_DIR/my_profile.txt" "Мой профиль"

log "=== Обновление профиля ==="
UPDATE_PROFILE_JSON='{"fullName":"User One Updated","description":"Updated description","experience":5}'
curl -s -w "\n%{http_code}" -X PATCH "$BASE_URL/api/profiles/me" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN1" \
  -d "$UPDATE_PROFILE_JSON" > "$TMP_DIR/update_profile.txt"
dump_response "$TMP_DIR/update_profile.txt" "Обновление профиля"

# Проверка обновления
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/profiles/me" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/my_profile_after.txt"
dump_response "$TMP_DIR/my_profile_after.txt" "Профиль после обновления"

# ---------- Избранное ----------
log "=== Добавление в избранное (первый профиль -> второй) ==="
IDEMPOTENCY_KEY=$(date +%s%N)
curl -s -w "\n%{http_code}" -X PUT "$BASE_URL/api/$PROFILE2_ID/favorite" \
  -H "Authorization: Bearer $TOKEN1" \
  -H "Idempotency-Key: $IDEMPOTENCY_KEY" > "$TMP_DIR/fav_add.txt"
dump_response "$TMP_DIR/fav_add.txt" "Добавление в избранное"

log "=== Попытка повторного добавления (ожидаем 409) ==="
IDEMPOTENCY_KEY2=$(date +%s%N)
curl -s -w "\n%{http_code}" -X PUT "$BASE_URL/api/$PROFILE2_ID/favorite" \
  -H "Authorization: Bearer $TOKEN1" \
  -H "Idempotency-Key: $IDEMPOTENCY_KEY2" > "$TMP_DIR/fav_add_dup.txt"
dump_response "$TMP_DIR/fav_add_dup.txt" "Повторное добавление (должен быть 409)"

log "=== Получение списка избранного ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/me/favorites?page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/fav_list.txt"
dump_response "$TMP_DIR/fav_list.txt" "Избранное"

log "=== Удаление из избранного ==="
curl -s -w "\n%{http_code}" -X DELETE "$BASE_URL/api/$PROFILE2_ID/favorite" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/fav_remove.txt"
dump_response "$TMP_DIR/fav_remove.txt" "Удаление из избранного"

# Проверка, что флаг сброшен
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/profiles/$PROFILE2_ID" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/profile_after_fav_remove.txt"
dump_response "$TMP_DIR/profile_after_fav_remove.txt" "Профиль после удаления из избранного"

# ---------- Мероприятия ----------
log "=== Создание мероприятия ==="
START_DATE=$(date -u -d "+7 days" +"%Y-%m-%dT%H:%M:%SZ")
EVENT_JSON=$("$JQ" -n --arg start "$START_DATE" '{
  title: "Test Event",
  description: "Event for testing registration",
  regionId: 1,
  cityId: 1,
  address: "Test Address, 123",
  startDateTime: $start,
  maxParticipants: 10
}')

IDEMPOTENCY_KEY_EVENT=$(date +%s%N)
curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/events" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN1" \
  -H "Idempotency-Key: $IDEMPOTENCY_KEY_EVENT" \
  -d "$EVENT_JSON" > "$TMP_DIR/event_create.txt"
dump_response "$TMP_DIR/event_create.txt" "Создание мероприятия"
EVENT_BODY=$(sed '$d' "$TMP_DIR/event_create.txt")
EVENT_ID=$("$JQ" -r '.id' <<< "$EVENT_BODY")
log "ID мероприятия: $EVENT_ID"

log "=== Получение мероприятия по ID ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/events/$EVENT_ID" \
  -H "Authorization: Bearer $TOKEN2" > "$TMP_DIR/event_get.txt"
dump_response "$TMP_DIR/event_get.txt" "Мероприятие по ID"

log "=== Список мероприятий ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/events?page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN2" > "$TMP_DIR/events_list.txt"
dump_response "$TMP_DIR/events_list.txt" "Список мероприятий"

log "=== Созданные мероприятия (первый пользователь) ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/events/created?page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/events_created.txt"
dump_response "$TMP_DIR/events_created.txt" "Созданные мероприятия"

log "=== Регистрация второго пользователя ==="
IDEMPOTENCY_KEY_REG=$(date +%s%N)
curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/events/$EVENT_ID/registration" \
  -H "Authorization: Bearer $TOKEN2" \
  -H "Idempotency-Key: $IDEMPOTENCY_KEY_REG" > "$TMP_DIR/event_reg.txt"
dump_response "$TMP_DIR/event_reg.txt" "Регистрация на мероприятие"

log "=== Зарегистрированные мероприятия (второй пользователь) ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/events/registered?page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN2" > "$TMP_DIR/events_registered.txt"
dump_response "$TMP_DIR/events_registered.txt" "Зарегистрированные мероприятия"

log "=== Попытка регистрации создателя (ожидаем ошибку) ==="
curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/events/$EVENT_ID/registration" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/event_reg_creator.txt"
dump_response "$TMP_DIR/event_reg_creator.txt" "Регистрация создателя (должна быть ошибка)"

log "=== Отмена регистрации второго пользователя ==="
curl -s -w "\n%{http_code}" -X DELETE "$BASE_URL/api/events/$EVENT_ID/registration" \
  -H "Authorization: Bearer $TOKEN2" > "$TMP_DIR/event_unreg.txt"
dump_response "$TMP_DIR/event_unreg.txt" "Отмена регистрации"

log "=== Обновление мероприятия ==="
UPDATE_EVENT_JSON='{"title":"Updated Test Event","description":"Updated description"}'
curl -s -w "\n%{http_code}" -X PATCH "$BASE_URL/api/events/$EVENT_ID" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN1" \
  -d "$UPDATE_EVENT_JSON" > "$TMP_DIR/event_update.txt"
dump_response "$TMP_DIR/event_update.txt" "Обновление мероприятия"

log "=== Отмена мероприятия ==="
curl -s -w "\n%{http_code}" -X DELETE "$BASE_URL/api/events/$EVENT_ID" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/event_cancel.txt"
dump_response "$TMP_DIR/event_cancel.txt" "Отмена мероприятия"

# ---------- Предложения ----------
log "=== Отправка предложения о сотрудничестве (второй -> первый) ==="
SUGGEST_JSON=$("$JQ" -n --arg pid "$PROFILE1_ID" '{
  toProfileId: $pid,
  message: "Test collaboration suggestion"
}')
IDEMPOTENCY_KEY_SUGG=$(date +%s%N)
curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/suggestions" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN2" \
  -H "Idempotency-Key: $IDEMPOTENCY_KEY_SUGG" \
  -d "$SUGGEST_JSON" > "$TMP_DIR/sugg_send.txt"
dump_response "$TMP_DIR/sugg_send.txt" "Отправка предложения"
SUGG_BODY=$(sed '$d' "$TMP_DIR/sugg_send.txt")
SUGGESTION_ID=$("$JQ" -r '.id' <<< "$SUGG_BODY" 2>/dev/null)  # может быть нет, если 204

log "=== Входящие предложения (первый пользователь) ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/suggestions/received?page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/sugg_received.txt"
dump_response "$TMP_DIR/sugg_received.txt" "Входящие предложения"
RECEIVED_JSON=$(sed '$d' "$TMP_DIR/sugg_received.txt")
SUGG_ID=$("$JQ" -r '.items[0].id' <<< "$RECEIVED_JSON")

log "=== Исходящие предложения (второй пользователь) ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/suggestions/sent?page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN2" > "$TMP_DIR/sugg_sent.txt"
dump_response "$TMP_DIR/sugg_sent.txt" "Исходящие предложения"

if [ -n "$SUGG_ID" ] && [ "$SUGG_ID" != "null" ]; then
  log "=== Принятие предложения ==="
  ACCEPT_JSON='{"status":"Accepted"}'
  curl -s -w "\n%{http_code}" -X PATCH "$BASE_URL/api/suggestions/$SUGG_ID/status" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN1" \
    -d "$ACCEPT_JSON" > "$TMP_DIR/sugg_accept.txt"
  dump_response "$TMP_DIR/sugg_accept.txt" "Принятие предложения"
else
  log "Невозможно определить ID предложения, пропускаем принятие"
fi

# ---------- Уведомления ----------
log "=== Уведомления первого пользователя ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/notifications?page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/notif_list1.txt"
dump_response "$TMP_DIR/notif_list1.txt" "Уведомления пользователя 1"
NOTIF1_BODY=$(sed '$d' "$TMP_DIR/notif_list1.txt")
NOTIF_ID=$("$JQ" -r '.items[0].id' <<< "$NOTIF1_BODY")

log "=== Количество непрочитанных уведомлений ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/notifications/unread-count" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/unread_count.txt"
dump_response "$TMP_DIR/unread_count.txt" "Непрочитанные"

if [ -n "$NOTIF_ID" ] && [ "$NOTIF_ID" != "null" ]; then
  log "=== Отметка одного уведомления как прочитанного ==="
  curl -s -w "\n%{http_code}" -X PATCH "$BASE_URL/api/notifications/$NOTIF_ID/read" \
    -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/notif_mark_read.txt"
  dump_response "$TMP_DIR/notif_mark_read.txt" "Отметка прочитанным"
fi

log "=== Отметка всех уведомлений как прочитанных ==="
curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/notifications/read-all" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/notif_read_all.txt"
dump_response "$TMP_DIR/notif_read_all.txt" "Отметка всех"

log "=== Обновление настроек уведомлений ==="
SETTINGS_JSON='{"notifyByEmail":false,"notifyByVk":true}'
curl -s -w "\n%{http_code}" -X PATCH "$BASE_URL/api/notifications/settings" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN1" \
  -d "$SETTINGS_JSON" > "$TMP_DIR/notif_settings.txt"
dump_response "$TMP_DIR/notif_settings.txt" "Обновление настроек"

log "=== Получение настроек уведомлений ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/notifications/settings" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/notif_get_settings.txt"
dump_response "$TMP_DIR/notif_get_settings.txt" "Настройки уведомлений"

# ---------- Удаление профиля (третий пользователь) ----------
log "=== Удаление профиля третьего пользователя ==="
curl -s -w "\n%{http_code}" -X DELETE "$BASE_URL/api/profiles/me" \
  -H "Authorization: Bearer $TOKEN3" > "$TMP_DIR/profile_delete.txt"
dump_response "$TMP_DIR/profile_delete.txt" "Удаление профиля"

log "=== Проверка, что профиль удалён (должен вернуть 404) ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/profiles/$PROFILE3_ID" \
  -H "Authorization: Bearer $TOKEN2" > "$TMP_DIR/profile_deleted_check.txt"
dump_response "$TMP_DIR/profile_deleted_check.txt" "Поиск удалённого профиля"

# ---------- Текущий пользователь ----------
log "=== Получение информации о текущем пользователе ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/user" \
  -H "Authorization: Bearer $TOKEN1" > "$TMP_DIR/current_user.txt"
dump_response "$TMP_DIR/current_user.txt" "Текущий пользователь"

log "=== Полный тест завершён ==="
echo "Лог сохранён в $LOG_FILE"