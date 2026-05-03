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

FAILED_TESTS=()
PASSED_TESTS=0

check_test() {
  local description="$1"
  local expected="$2"
  local actual="$3"
  local success_msg="$4"
  local fail_msg="$5"
  if [ "$actual" = "$expected" ]; then
    log "✓ $description ($success_msg)"
    PASSED_TESTS=$((PASSED_TESTS + 1))
  else
    log "✗ $description ($fail_msg: ожидалось $expected, получено $actual)"
    FAILED_TESTS+=("$description")
  fi
}

# Универсальная функция запроса с логированием
curl_log() {
  local method="$1"
  local url="$2"
  local headers="$3"
  local data="$4"
  local output_file="$5"

  log "Запрос: $method $url"
  if [ -n "$data" ]; then
    log "Тело запроса: $data"
  fi

  local cmd="curl -s -w '%{http_code}' -X $method '$url'"
  if [ -n "$headers" ]; then
    cmd="$cmd $headers"
  fi
  if [ -n "$data" ]; then
    cmd="$cmd -d '$data'"
  fi
  cmd="$cmd > '$output_file'"

  eval "$cmd"

  local raw=$(<"$output_file")
  local len=${#raw}
  local body="${raw:0:$((len-3))}"
  local code="${raw:$((len-3))}"

  log "Ответ (HTTP $code): $body"

  echo "$body" > "$output_file.body"
  echo "$code" > "$output_file.code"
}

get_http_code() {
  local file="$1"
  cat "$file.code" 2>/dev/null || echo "000"
}

get_body() {
  local file="$1"
  cat "$file.body" 2>/dev/null
}

register_user() {
  local EMAIL="$1"
  local USER_NAME="$2"
  local PROFILE_EXTRA="$3"

  log "Регистрация $EMAIL ..."

  curl_log "POST" "$BASE_URL/api/auth/code" "-H 'Content-Type: application/json'" "{\"email\":\"$EMAIL\"}" "$TMP_DIR/code_$EMAIL.txt"

  curl_log "POST" "$BASE_URL/api/auth/session" "-H 'Content-Type: application/json'" "{\"email\":\"$EMAIL\",\"code\":\"$VERIFICATION_CODE\"}" "$TMP_DIR/login_$EMAIL.txt"

  local auth_body=$(get_body "$TMP_DIR/login_$EMAIL.txt")
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

  curl_log "POST" "$BASE_URL/api/profiles" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN'" "$profile_json" "$TMP_DIR/profile_$EMAIL.txt"

  local profile_body=$(get_body "$TMP_DIR/profile_$EMAIL.txt")
  local PROFILE_ID=$("$JQ" -r '.id' <<< "$profile_body")

  if [ "$PROFILE_ID" = "null" ] || [ -z "$PROFILE_ID" ]; then
    log "ОШИБКА: не удалось создать профиль"
    exit 1
  fi

  echo "$TOKEN $USER_ID $PROFILE_ID"
}

# ---------- Старт ----------
log "=== Запуск полного тестирования API ==="
log "Временная папка: $TMP_DIR"

docker-compose -f docker-compose.backend.yml down -v > /dev/null 2>&1
docker-compose -f docker-compose.backend.yml up --build -d

log "Ожидание доступности API..."
for i in $(seq 1 60); do
    code=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL/swagger/index.html")
    if [ "$code" = "200" ]; then
        log "API доступен"
        break
    fi
    sleep 2
done

# ---------- Справочные данные ----------
log "=== Справочные данные ==="
curl_log "GET" "$BASE_URL/api/metadata/cities" "" "" "$TMP_DIR/meta_cities.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/meta_cities.txt")
check_test "GET /api/metadata/cities (200)" "200" "$HTTP_CODE" "Справочник городов доступен" "Ошибка получения"

curl_log "GET" "$BASE_URL/api/metadata/regions" "" "" "$TMP_DIR/meta_regions.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/meta_regions.txt")
check_test "GET /api/metadata/regions (200)" "200" "$HTTP_CODE" "Справочник регионов доступен" "Ошибка получения"

curl_log "GET" "$BASE_URL/api/metadata/genres" "" "" "$TMP_DIR/meta_genres.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/meta_genres.txt")
check_test "GET /api/metadata/genres (200)" "200" "$HTTP_CODE" "Справочник жанров доступен" "Ошибка получения"

curl_log "GET" "$BASE_URL/api/metadata/specialties" "" "" "$TMP_DIR/meta_specialties.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/meta_specialties.txt")
check_test "GET /api/metadata/specialties (200)" "200" "$HTTP_CODE" "Справочник специальностей доступен" "Ошибка получения"

curl_log "GET" "$BASE_URL/api/metadata/goals" "" "" "$TMP_DIR/meta_goals.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/meta_goals.txt")
check_test "GET /api/metadata/goals (200)" "200" "$HTTP_CODE" "Справочник целей доступен" "Ошибка получения"

# ---------- Регистрация пользователей ----------
log "=== Регистрация пользователей ==="

USER1_DATA=$(register_user "user1@test.com" "User One" '{"genreIds":[1],"specialtyIds":[1],"collaborationGoalIds":[1],"desiredGenreIds":[2],"desiredSpecialtyIds":[2],"experience":5,"lookingFor":"LookingForMusician"}')
read TOKEN1 USER1_ID PROFILE1_ID <<< "$USER1_DATA"

USER2_DATA=$(register_user "user2@test.com" "User Two" '{"genreIds":[1,2],"specialtyIds":[2],"collaborationGoalIds":[2],"desiredGenreIds":[1],"desiredSpecialtyIds":[1],"cityId":2,"experience":10,"lookingFor":"LookingForBand"}')
read TOKEN2 USER2_ID PROFILE2_ID <<< "$USER2_DATA"

USER3_DATA=$(register_user "chmo228098@gmail.com" "User Three" '{}')
read TOKEN3 USER3_ID PROFILE3_ID <<< "$USER3_DATA"

# User4 (будет использован для валидации и негативных тестов)
log "Регистрация user4@test.com (без профиля)..."
curl_log "POST" "$BASE_URL/api/auth/code" "-H 'Content-Type: application/json'" '{"email":"user4@test.com"}' "$TMP_DIR/code_user4.txt"
curl_log "POST" "$BASE_URL/api/auth/session" "-H 'Content-Type: application/json'" '{"email":"user4@test.com","code":"111111"}' "$TMP_DIR/login_user4.txt"
TOKEN4=$(get_body "$TMP_DIR/login_user4.txt" | "$JQ" -r '.token')
USER4_ID=$(get_body "$TMP_DIR/login_user4.txt" | "$JQ" -r '.user.id')
log "User4: userId=$USER4_ID"

# ---------- Прямая установка VkUserId для User3 ----------
log "=== Установка VkUserId и настроек для User3 ==="
POSTGRES_CONTAINER=$(docker ps --filter "name=postgres" --format "{{.ID}}" | head -1)
if [ -n "$POSTGRES_CONTAINER" ]; then
  docker exec "$POSTGRES_CONTAINER" psql -U postgres -d musicianfinder -c "UPDATE \"MusicianProfile\" SET \"VkUserId\"='241302814' WHERE \"Id\"='$PROFILE3_ID';" > /dev/null 2>&1
  log "VkUserId 241302814 установлен для профиля $PROFILE3_ID"
else
  log "ОШИБКА: контейнер postgres не найден"
fi

curl_log "PATCH" "$BASE_URL/api/notifications/settings" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN3'" '{"notifyByEmail":true,"notifyByVk":true}' "$TMP_DIR/notif_settings_user3.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/notif_settings_user3.txt")
check_test "PATCH /api/notifications/settings User3 (204)" "204" "$HTTP_CODE" "Настройки обновлены" "Ошибка обновления для User3"

curl_log "GET" "$BASE_URL/api/notifications/settings" "-H 'Authorization: Bearer $TOKEN3'" "" "$TMP_DIR/notif_settings_user3_get.txt"
SETTINGS3_BODY=$(get_body "$TMP_DIR/notif_settings_user3_get.txt")
NOTIFY_EMAIL3=$("$JQ" -r '.notifyByEmail' <<< "$SETTINGS3_BODY")
NOTIFY_VK3=$("$JQ" -r '.notifyByVk' <<< "$SETTINGS3_BODY")
check_test "User3 notifyByEmail" "true" "$NOTIFY_EMAIL3" "Email включены" "Не включены"
check_test "User3 notifyByVk" "true" "$NOTIFY_VK3" "VK включены" "Не включены"

# ---------- Профили: получение и маппинг ----------
log "=== Профили ==="
curl_log "GET" "$BASE_URL/api/profiles/$PROFILE1_ID" "-H 'Authorization: Bearer $TOKEN2'" "" "$TMP_DIR/get_profile1.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/get_profile1.txt")
PROFILE1_BODY=$(get_body "$TMP_DIR/get_profile1.txt")
check_test "GET /api/profiles/{id} (200)" "200" "$HTTP_CODE" "Профиль получен" "Ошибка получения"

FULLNAME=$("$JQ" -r '.fullName' <<< "$PROFILE1_BODY")
check_test "Имя профиля" "User One" "$FULLNAME" "Имя корректно" "Не совпадает"

GENRES_LEN=$("$JQ" -r '.genres | length' <<< "$PROFILE1_BODY")
check_test "Количество жанров" "1" "$GENRES_LEN" "Жанры загружены" "Неверное число"

EXPERIENCE=$("$JQ" -r '.experience' <<< "$PROFILE1_BODY")
check_test "Опыт" "5" "$EXPERIENCE" "Опыт передан" "Не совпадает"

# ---------- Фильтрация профилей ----------
log "=== Фильтрация профилей (коллекции) ==="

# specialityIds (от User2, чтобы не исключать собственный профиль)
curl_log "GET" "$BASE_URL/api/profiles?specialtyIds=1&page=1&limit=10" "-H 'Authorization: Bearer $TOKEN2'" "" "$TMP_DIR/filter_specialty.txt"
BODY=$(get_body "$TMP_DIR/filter_specialty.txt")
ITEMS_COUNT=$("$JQ" -r '.items | length' <<< "$BODY")
check_test "Фильтр по специальности 1 (есть результат)" "1" "$([ $ITEMS_COUNT -gt 0 ] && echo 1 || echo 0)" "Найдены профили" "Нет результатов"

# collaborationGoalIds (от User2)
curl_log "GET" "$BASE_URL/api/profiles?goalIds=1&page=1&limit=10" "-H 'Authorization: Bearer $TOKEN2'" "" "$TMP_DIR/filter_goals.txt"
BODY=$(get_body "$TMP_DIR/filter_goals.txt")
ITEMS_COUNT=$("$JQ" -r '.items | length' <<< "$BODY")
check_test "Фильтр по цели сотрудничества 1 (есть результат)" "1" "$([ $ITEMS_COUNT -gt 0 ] && echo 1 || echo 0)" "Найдены профили" "Нет результатов"

# desiredGenreIds (от User2)
curl_log "GET" "$BASE_URL/api/profiles?desiredGenreIds=2&page=1&limit=10" "-H 'Authorization: Bearer $TOKEN2'" "" "$TMP_DIR/filter_desired_genre.txt"
BODY=$(get_body "$TMP_DIR/filter_desired_genre.txt")
ITEMS_COUNT=$("$JQ" -r '.items | length' <<< "$BODY")
check_test "Фильтр по искомому жанру 2 (есть результат)" "1" "$([ $ITEMS_COUNT -gt 0 ] && echo 1 || echo 0)" "Найдены профили" "Нет результатов"

# desiredSpecialtyIds (от User2)
curl_log "GET" "$BASE_URL/api/profiles?desiredSpecialtyIds=2&page=1&limit=10" "-H 'Authorization: Bearer $TOKEN2'" "" "$TMP_DIR/filter_desired_specialty.txt"
BODY=$(get_body "$TMP_DIR/filter_desired_specialty.txt")
ITEMS_COUNT=$("$JQ" -r '.items | length' <<< "$BODY")
check_test "Фильтр по искомой специальности 2 (есть результат)" "1" "$([ $ITEMS_COUNT -gt 0 ] && echo 1 || echo 0)" "Найдены профили" "Нет результатов"

# Остальные проверенные фильтры
curl_log "GET" "$BASE_URL/api/profiles?genreIds=1&page=1&limit=10" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/filter_genre.txt"
BODY=$(get_body "$TMP_DIR/filter_genre.txt")
ITEMS_COUNT=$("$JQ" -r '.items | length' <<< "$BODY")
check_test "Фильтр по жанру 1 (кол-во > 0)" "1" "$([ $ITEMS_COUNT -gt 0 ] && echo 1 || echo 0)" "Найдены профили" "Нет результатов"

curl_log "GET" "$BASE_URL/api/profiles?cityId=2&page=1&limit=10" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/filter_city.txt"
BODY=$(get_body "$TMP_DIR/filter_city.txt")
CITY_NAME=$("$JQ" -r '.items[0].city.name' <<< "$BODY")
check_test "Фильтр по городу 2 (Saint Petersburg)" "Saint Petersburg" "$CITY_NAME" "Город совпадает" "Не совпадает"

curl_log "GET" "$BASE_URL/api/profiles?experienceMin=5&page=1&limit=10" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/filter_exp.txt"
BODY=$(get_body "$TMP_DIR/filter_exp.txt")
ITEMS_COUNT=$("$JQ" -r '.items | length' <<< "$BODY")
check_test "Фильтр по опыту >=5 (найдены профили)" "1" "$([ $ITEMS_COUNT -gt 0 ] && echo 1 || echo 0)" "Профили с опытом найдены" "Нет результатов"

curl_log "GET" "$BASE_URL/api/profiles?lookingFor=LookingForBand&page=1&limit=10" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/filter_looking.txt"
BODY=$(get_body "$TMP_DIR/filter_looking.txt")
LF_NAME=$("$JQ" -r '.items[0].lookingFor' <<< "$BODY")
check_test "Фильтр LookingForBand" "LookingForBand" "$LF_NAME" "Значение LookingFor совпадает" "Не совпадает"

curl_log "GET" "$BASE_URL/api/profiles?genreIds=2&cityId=2&page=1&limit=10" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/filter_multi.txt"
BODY=$(get_body "$TMP_DIR/filter_multi.txt")
ITEMS_COUNT=$("$JQ" -r '.items | length' <<< "$BODY")
check_test "Фильтр жанр=2 и город=2 (есть результат)" "1" "$([ $ITEMS_COUNT -gt 0 ] && echo 1 || echo 0)" "Найден профиль" "Нет результатов"

# ---------- Обновление профиля ----------
log "=== Обновление профиля ==="
UPDATE_JSON='{"fullName":"User One Updated","description":"Updated","experience":7,"lookingFor":"LookingForBand"}'
curl_log "PATCH" "$BASE_URL/api/profiles/me" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN1'" "$UPDATE_JSON" "$TMP_DIR/update_profile.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/update_profile.txt")
check_test "PATCH /api/profiles/me (200)" "200" "$HTTP_CODE" "Профиль обновлён" "Ошибка обновления"

curl_log "GET" "$BASE_URL/api/profiles/me" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/my_profile_after.txt"
PROFILE_AFTER=$(get_body "$TMP_DIR/my_profile_after.txt")
NEW_NAME=$("$JQ" -r '.fullName' <<< "$PROFILE_AFTER")
check_test "Обновлённое имя" "User One Updated" "$NEW_NAME" "Имя изменено" "Не изменилось"
NEW_EXP=$("$JQ" -r '.experience' <<< "$PROFILE_AFTER")
check_test "Обновлённый опыт" "7" "$NEW_EXP" "Опыт изменён" "Не изменился"

# ---------- Избранное ----------
log "=== Избранное ==="
IDEMPOTENCY_KEY=$(date +%s%N)
curl_log "PUT" "$BASE_URL/api/$PROFILE2_ID/favorite" "-H 'Authorization: Bearer $TOKEN1' -H 'Idempotency-Key: $IDEMPOTENCY_KEY'" "" "$TMP_DIR/fav_add.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/fav_add.txt")
check_test "PUT /api/{profileId}/favorite (204)" "204" "$HTTP_CODE" "Добавлено" "Ошибка добавления"

curl_log "GET" "$BASE_URL/api/profiles/$PROFILE2_ID" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/profile2_fav.txt"
IS_FAV=$("$JQ" -r '.isFavorite' <<< "$(get_body "$TMP_DIR/profile2_fav.txt")")
check_test "Флаг isFavorite" "true" "$IS_FAV" "Флаг установлен" "Не установлен"

curl_log "GET" "$BASE_URL/api/me/favorites" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/fav_list.txt"
FAV_LIST_BODY=$(get_body "$TMP_DIR/fav_list.txt")
FAV_COUNT=$("$JQ" -r '.total' <<< "$FAV_LIST_BODY")
check_test "Список избранного (total=1)" "1" "$FAV_COUNT" "Один профиль в избранном" "Неверное количество"

curl_log "DELETE" "$BASE_URL/api/$PROFILE2_ID/favorite" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/fav_remove.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/fav_remove.txt")
check_test "DELETE /api/{profileId}/favorite (204)" "204" "$HTTP_CODE" "Удалено" "Ошибка удаления"

curl_log "GET" "$BASE_URL/api/profiles/$PROFILE2_ID" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/profile2_fav_after.txt"
IS_FAV_AFTER=$("$JQ" -r '.isFavorite' <<< "$(get_body "$TMP_DIR/profile2_fav_after.txt")")
check_test "Флаг isFavorite после удаления" "false" "$IS_FAV_AFTER" "Флаг сброшен" "Не сброшен"

# ---------- Мероприятия ----------
log "=== Мероприятия ==="
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
curl_log "POST" "$BASE_URL/api/events" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN1' -H 'Idempotency-Key: $IDEMPOTENCY_KEY'" "$EVENT1_JSON" "$TMP_DIR/event1.txt"
EVENT1_ID=$("$JQ" -r '.id' <<< "$(get_body "$TMP_DIR/event1.txt")")
HTTP_CODE=$(get_http_code "$TMP_DIR/event1.txt")
check_test "POST /api/events (201)" "201" "$HTTP_CODE" "Мероприятие создано" "Ошибка создания"

curl_log "GET" "$BASE_URL/api/events/$EVENT1_ID" "" "" "$TMP_DIR/event1_get.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/event1_get.txt")
check_test "GET /api/events/{id} (200)" "200" "$HTTP_CODE" "Мероприятие получено" "Ошибка получения"

# ---------- Фильтрация мероприятий ----------
log "=== Фильтрация мероприятий ==="
curl_log "GET" "$BASE_URL/api/events?cityId=1" "" "" "$TMP_DIR/events_city1.txt"
BODY=$(get_body "$TMP_DIR/events_city1.txt")
ITEMS_COUNT=$("$JQ" -r '.items | length' <<< "$BODY")
check_test "Фильтр по городу 1 (есть результат)" "1" "$([ $ITEMS_COUNT -gt 0 ] && echo 1 || echo 0)" "Найдены мероприятия" "Нет результатов"

curl_log "GET" "$BASE_URL/api/events?regionId=1" "" "" "$TMP_DIR/events_region1.txt"
BODY=$(get_body "$TMP_DIR/events_region1.txt")
ITEMS_COUNT=$("$JQ" -r '.items | length' <<< "$BODY")
check_test "Фильтр по региону 1 (есть результат)" "1" "$([ $ITEMS_COUNT -gt 0 ] && echo 1 || echo 0)" "Найдены мероприятия" "Нет результатов"

curl_log "GET" "$BASE_URL/api/events?query=Jazz" "" "" "$TMP_DIR/events_query.txt"
BODY=$(get_body "$TMP_DIR/events_query.txt")
ITEMS_COUNT=$("$JQ" -r '.items | length' <<< "$BODY")
check_test "Поиск по названию (есть результат)" "1" "$([ $ITEMS_COUNT -gt 0 ] && echo 1 || echo 0)" "Найдены мероприятия" "Нет результатов"

# ---------- Регистрация User2 и User3 на мероприятие ----------
log "=== Регистрация ==="
curl_log "POST" "$BASE_URL/api/events/$EVENT1_ID/registration" "-H 'Authorization: Bearer $TOKEN2'" "" "$TMP_DIR/event_reg_user2.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/event_reg_user2.txt")
check_test "POST /api/events/{id}/registration User2 (204)" "204" "$HTTP_CODE" "Регистрация успешна" "Ошибка регистрации"

curl_log "POST" "$BASE_URL/api/events/$EVENT1_ID/registration" "-H 'Authorization: Bearer $TOKEN3'" "" "$TMP_DIR/event_reg_user3.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/event_reg_user3.txt")
check_test "POST /api/events/{id}/registration User3 (204)" "204" "$HTTP_CODE" "Регистрация успешна" "Ошибка регистрации"

# ---------- Отправка предложения User2 -> User3 ----------
log "=== Предложение User2 -> User3 ==="
curl_log "POST" "$BASE_URL/api/suggestions" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN2'" "{\"toProfileId\":\"$PROFILE3_ID\",\"message\":\"Hi\"}" "$TMP_DIR/sugg_user3.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/sugg_user3.txt")
check_test "POST /api/suggestions User2->User3 (204)" "204" "$HTTP_CODE" "Предложение отправлено" "Ошибка отправки"

log "Ожидание обработки Outbox (15 секунд)..."
sleep 15

curl_log "GET" "$BASE_URL/api/notifications?page=1&limit=10" "-H 'Authorization: Bearer $TOKEN3'" "" "$TMP_DIR/notif_user3.txt"
NOTIF3_BODY=$(get_body "$TMP_DIR/notif_user3.txt")
NOTIF3_COUNT=$("$JQ" -r '.items | length' <<< "$NOTIF3_BODY")
check_test "Уведомления User3 (количество >= 2)" "1" "$([ $NOTIF3_COUNT -ge 2 ] && echo 1 || echo 0)" "Есть CollaborationReceived и EventRegistration" "Недостаточно уведомлений"

# ---------- Проверка созданных/зарегистрированных мероприятий ----------
log "=== Проверка созданных/зарегистрированных мероприятий ==="
curl_log "GET" "$BASE_URL/api/events/created?page=1&limit=10" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/events_created.txt"
CREATED_BODY=$(get_body "$TMP_DIR/events_created.txt")
CREATED_COUNT=$("$JQ" -r '.total' <<< "$CREATED_BODY")
CREATED_ISCREATOR=$("$JQ" -r '.items[0].isCreator' <<< "$CREATED_BODY")
check_test "Созданные мероприятия (total=1)" "1" "$CREATED_COUNT" "Есть созданное мероприятие" "Неверное количество"
check_test "isCreator в созданных" "true" "$CREATED_ISCREATOR" "Флаг isCreator установлен" "Не установлен"

curl_log "GET" "$BASE_URL/api/events/registered?page=1&limit=10" "-H 'Authorization: Bearer $TOKEN2'" "" "$TMP_DIR/events_registered.txt"
REG_BODY=$(get_body "$TMP_DIR/events_registered.txt")
REG_TOTAL=$("$JQ" -r '.total' <<< "$REG_BODY")
check_test "Зарегистрированные мероприятия (total=1)" "1" "$REG_TOTAL" "Есть зарегистрированное мероприятие" "Неверное количество"
REG_ISREG=$("$JQ" -r '.items[0].isRegistered' <<< "$REG_BODY")
check_test "isRegistered в registered-list" "true" "$REG_ISREG" "Флаг isRegistered установлен" "Не установлен"

# ---------- Проверка флагов на конкретном мероприятии ----------
log "=== Проверка флагов ==="
curl_log "GET" "$BASE_URL/api/events/$EVENT1_ID" "-H 'Authorization: Bearer $TOKEN2'" "" "$TMP_DIR/event1_user2.txt"
EVENT_VIEW=$(get_body "$TMP_DIR/event1_user2.txt")
IS_REG=$("$JQ" -r '.isRegistered' <<< "$EVENT_VIEW")
check_test "isRegistered User2" "true" "$IS_REG" "Зарегистрирован" "Не зарегистрирован"
CURR_PART=$("$JQ" -r '.currentParticipants' <<< "$EVENT_VIEW")
check_test "currentParticipants >= 2" "1" "$([ $CURR_PART -ge 2 ] && echo 1 || echo 0)" "Участники учтены" "Меньше 2"

# Отмена регистрации User2
curl_log "DELETE" "$BASE_URL/api/events/$EVENT1_ID/registration" "-H 'Authorization: Bearer $TOKEN2'" "" "$TMP_DIR/event_unreg.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/event_unreg.txt")
check_test "DELETE /api/events/{id}/registration (204)" "204" "$HTTP_CODE" "Регистрация отменена" "Ошибка отмены"

# ---------- Обновление мероприятия ----------
log "=== Обновление мероприятия ==="
UPDATE_EVENT_JSON='{"title":"Updated Jazz","description":"New desc"}'
curl_log "PATCH" "$BASE_URL/api/events/$EVENT1_ID" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN1'" "$UPDATE_EVENT_JSON" "$TMP_DIR/event_update.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/event_update.txt")
check_test "PATCH /api/events/{id} (200)" "200" "$HTTP_CODE" "Мероприятие обновлено" "Ошибка обновления"

curl_log "GET" "$BASE_URL/api/events/$EVENT1_ID" "" "" "$TMP_DIR/event_after_update.txt"
UPDATED_TITLE=$("$JQ" -r '.title' <<< "$(get_body "$TMP_DIR/event_after_update.txt")")
check_test "Обновлённое название" "Updated Jazz" "$UPDATED_TITLE" "Название обновлено" "Не обновлено"

# ---------- Отмена мероприятия ----------
log "=== Отмена мероприятия ==="
curl_log "DELETE" "$BASE_URL/api/events/$EVENT1_ID" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/event_cancel.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/event_cancel.txt")
check_test "DELETE /api/events/{id} (204)" "204" "$HTTP_CODE" "Мероприятие отменено" "Ошибка отмены"

curl_log "GET" "$BASE_URL/api/events/$EVENT1_ID" "" "" "$TMP_DIR/event_cancelled_status.txt"
STATUS=$("$JQ" -r '.status' <<< "$(get_body "$TMP_DIR/event_cancelled_status.txt")")
check_test "Статус Cancelled" "Cancelled" "$STATUS" "Статус изменился" "Не Cancelled"

curl_log "GET" "$BASE_URL/api/events?status=Cancelled" "" "" "$TMP_DIR/events_cancelled_list.txt"
BODY=$(get_body "$TMP_DIR/events_cancelled_list.txt")
ITEMS_COUNT=$("$JQ" -r '.items | length' <<< "$BODY")
check_test "Фильтр по статусу Cancelled (есть результат)" "1" "$([ $ITEMS_COUNT -gt 0 ] && echo 1 || echo 0)" "Найдено отменённое мероприятие" "Нет результатов"

# ---------- Предложения User2 -> User1 ----------
log "=== Предложение User2 -> User1 ==="
curl_log "POST" "$BASE_URL/api/suggestions" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN2'" "{\"toProfileId\":\"$PROFILE1_ID\",\"message\":\"Hi\"}" "$TMP_DIR/sugg_user1.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/sugg_user1.txt")
check_test "POST /api/suggestions (204)" "204" "$HTTP_CODE" "Предложение отправлено" "Ошибка отправки"

curl_log "GET" "$BASE_URL/api/notifications?page=1&limit=10" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/notif_user1.txt"
NOTIF_BODY=$(get_body "$TMP_DIR/notif_user1.txt")
NOTIF_TYPE=$("$JQ" -r '.items[0].type' <<< "$NOTIF_BODY")
check_test "Уведомление CollaborationReceived" "CollaborationReceived" "$NOTIF_TYPE" "Уведомление получено" "Не получено"

SUGG_ID=$(curl -s "$BASE_URL/api/suggestions/received?page=1&limit=10" -H "Authorization: Bearer $TOKEN1" | "$JQ" -r '.items[0].id')
if [ -n "$SUGG_ID" ] && [ "$SUGG_ID" != "null" ]; then
  curl_log "PATCH" "$BASE_URL/api/suggestions/$SUGG_ID/status" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN1'" '{"status":"Accepted"}' "$TMP_DIR/sugg_accept.txt"
  HTTP_CODE=$(get_http_code "$TMP_DIR/sugg_accept.txt")
  check_test "PATCH /api/suggestions/{id}/status (204)" "204" "$HTTP_CODE" "Предложение принято" "Ошибка принятия"
fi

# ---------- Уведомления (настройки User1) ----------
log "=== Настройки уведомлений ==="
curl_log "GET" "$BASE_URL/api/notifications/settings" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/notif_settings_get.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/notif_settings_get.txt")
check_test "GET /api/notifications/settings (200)" "200" "$HTTP_CODE" "Настройки получены" "Ошибка получения"

curl_log "PATCH" "$BASE_URL/api/notifications/settings" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN1'" '{"notifyByEmail":false,"notifyByVk":true}' "$TMP_DIR/notif_settings_upd.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/notif_settings_upd.txt")
check_test "PATCH /api/notifications/settings (204)" "204" "$HTTP_CODE" "Настройки обновлены" "Ошибка обновления"

curl_log "GET" "$BASE_URL/api/notifications/settings" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/notif_settings_after.txt"
SETTINGS_BODY=$(get_body "$TMP_DIR/notif_settings_after.txt")
NOTIFY_EMAIL=$("$JQ" -r '.notifyByEmail' <<< "$SETTINGS_BODY")
check_test "notifyByEmail" "false" "$NOTIFY_EMAIL" "Отключены" "Не отключены"
NOTIFY_VK=$("$JQ" -r '.notifyByVk' <<< "$SETTINGS_BODY")
check_test "notifyByVk" "true" "$NOTIFY_VK" "Включены" "Не включены"

# ============================================================
# ========== НЕГАТИВНЫЕ ТЕСТЫ =================================
# ============================================================
log "=== Негативные тесты ==="

# --- Профили ---
curl_log "POST" "$BASE_URL/api/profiles" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN4'" '{"fullName":"Name","profileType":"Individual"}' "$TMP_DIR/neg_no_city.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_no_city.txt")
check_test "Профиль без cityId (400)" "400" "$HTTP_CODE" "Ошибка валидации" "Неверный статус"

curl_log "POST" "$BASE_URL/api/profiles" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN4'" '{"fullName":"","profileType":"Individual","cityId":1}' "$TMP_DIR/neg_empty_name.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_empty_name.txt")
check_test "Профиль с пустым именем (400)" "400" "$HTTP_CODE" "Ошибка валидации" "Неверный статус"

# Отрицательный опыт
curl_log "POST" "$BASE_URL/api/profiles" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN4'" '{"fullName":"Neg","profileType":"Individual","cityId":1,"experience":-1}' "$TMP_DIR/neg_neg_exp.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_neg_exp.txt")
check_test "Профиль с отрицательным опытом (400)" "400" "$HTTP_CODE" "Ошибка валидации" "Неверный статус"

# Несуществующий cityId
curl_log "POST" "$BASE_URL/api/profiles" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN4'" '{"fullName":"Neg","profileType":"Individual","cityId":9999,"experience":0}' "$TMP_DIR/neg_bad_city.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_bad_city.txt")
check_test "Профиль с несуществующим cityId (400)" "400" "$HTTP_CODE" "Ошибка валидации" "Неверный статус"

# Неверный ProfileType
curl_log "POST" "$BASE_URL/api/profiles" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN4'" '{"fullName":"Neg","profileType":"Invalid","cityId":1}' "$TMP_DIR/neg_bad_type.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_bad_type.txt")
check_test "Профиль с неверным ProfileType (400)" "400" "$HTTP_CODE" "Ошибка валидации" "Неверный статус"

# Поиск профилей: page=0
curl_log "GET" "$BASE_URL/api/profiles?page=0" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/neg_page0.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_page0.txt")
check_test "Поиск профилей page=0 (400)" "400" "$HTTP_CODE" "Ошибка валидации" "Неверный статус"

# limit=101
curl_log "GET" "$BASE_URL/api/profiles?limit=101" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/neg_limit101.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_limit101.txt")
check_test "Поиск профилей limit=101 (400)" "400" "$HTTP_CODE" "Ошибка валидации" "Неверный статус"

# --- Мероприятия ---
# Без address
curl_log "POST" "$BASE_URL/api/events" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN1'" '{"title":"NoAddr","regionId":1,"cityId":1,"startDateTime":"2027-01-01T00:00:00Z"}' "$TMP_DIR/neg_no_addr.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_no_addr.txt")
check_test "Мероприятие без address (400)" "400" "$HTTP_CODE" "Ошибка валидации" "Неверный статус"

# Отрицательное maxParticipants
curl_log "POST" "$BASE_URL/api/events" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN1'" '{"title":"NegMax","regionId":1,"cityId":1,"address":"Test","startDateTime":"2027-01-01T00:00:00Z","maxParticipants":-1}' "$TMP_DIR/neg_neg_max.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_neg_max.txt")
check_test "Мероприятие с отрицательным maxParticipants (400)" "400" "$HTTP_CODE" "Ошибка валидации" "Неверный статус"

# EndDateTime < StartDateTime
curl_log "POST" "$BASE_URL/api/events" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN1'" '{"title":"NegDate","regionId":1,"cityId":1,"address":"Test","startDateTime":"2027-01-01T00:00:00Z","endDateTime":"2026-01-01T00:00:00Z"}' "$TMP_DIR/neg_end_before_start.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_end_before_start.txt")
check_test "Мероприятие EndDateTime < StartDateTime (400)" "400" "$HTTP_CODE" "Ошибка валидации" "Неверный статус"

# Обновление не создателем
curl_log "PATCH" "$BASE_URL/api/events/$EVENT1_ID" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN2'" '{"title":"Hacked"}' "$TMP_DIR/neg_not_creator.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_not_creator.txt")
check_test "Обновление мероприятия не создателем (403)" "403" "$HTTP_CODE" "Доступ запрещён" "Неверный статус"

# Обновление отменённого мероприятия
curl_log "PATCH" "$BASE_URL/api/events/$EVENT1_ID" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN1'" '{"title":"Hacked"}' "$TMP_DIR/neg_cancelled_update.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_cancelled_update.txt")
check_test "Обновление отменённого мероприятия (400)" "400" "$HTTP_CODE" "Ошибка домена" "Неверный статус"

# Отмена мероприятия не создателем – пока ожидаем 403 (текущее поведение)
curl_log "DELETE" "$BASE_URL/api/events/$EVENT1_ID" "-H 'Authorization: Bearer $TOKEN2'" "" "$TMP_DIR/neg_cancel_not_creator.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_cancel_not_creator.txt")
check_test "Отмена мероприятия не создателем (403)" "403" "$HTTP_CODE" "Ошибка домена" "Неверный статус"

# Регистрация на отменённое мероприятие – User2 (имеет профиль, не зарегистрирован после отмены)
curl_log "POST" "$BASE_URL/api/events/$EVENT1_ID/registration" "-H 'Authorization: Bearer $TOKEN2'" "" "$TMP_DIR/neg_reg_cancelled.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_reg_cancelled.txt")
check_test "Регистрация на отменённое мероприятие (400)" "400" "$HTTP_CODE" "Ошибка домена" "Неверный статус"

# Регистрация создателем
curl_log "POST" "$BASE_URL/api/events/$EVENT1_ID/registration" "-H 'Authorization: Bearer $TOKEN1'" "" "$TMP_DIR/neg_reg_creator.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_reg_creator.txt")
check_test "Регистрация создателем (400)" "400" "$HTTP_CODE" "Ошибка домена" "Неверный статус"

# Отмена регистрации не зарегистрированным – User2
curl_log "DELETE" "$BASE_URL/api/events/$EVENT1_ID/registration" "-H 'Authorization: Bearer $TOKEN2'" "" "$TMP_DIR/neg_unreg_not_reg.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_unreg_not_reg.txt")
check_test "Отмена регистрации не зарегистрированным (400)" "400" "$HTTP_CODE" "Ошибка домена" "Неверный статус"

# Поиск мероприятий: page=0
curl_log "GET" "$BASE_URL/api/events?page=0" "" "" "$TMP_DIR/neg_ev_page0.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_ev_page0.txt")
check_test "Поиск мероприятий page=0 (400)" "400" "$HTTP_CODE" "Ошибка валидации" "Неверный статус"

# limit=101
curl_log "GET" "$BASE_URL/api/events?limit=101" "" "" "$TMP_DIR/neg_ev_limit101.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_ev_limit101.txt")
check_test "Поиск мероприятий limit=101 (400)" "400" "$HTTP_CODE" "Ошибка валидации" "Неверный статус"

# sortBy с неверным значением
curl_log "GET" "$BASE_URL/api/events?sortBy=invalid" "" "" "$TMP_DIR/neg_ev_sort.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_ev_sort.txt")
check_test "Поиск мероприятий sortBy=invalid (400)" "400" "$HTTP_CODE" "Ошибка валидации" "Неверный статус"

# --- Предложения ---
# Без авторизации
curl_log "POST" "$BASE_URL/api/suggestions" "-H 'Content-Type: application/json'" "{\"toProfileId\":\"$PROFILE1_ID\",\"message\":\"Hi\"}" "$TMP_DIR/neg_sugg_noauth.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_sugg_noauth.txt")
check_test "Предложение без авторизации (401)" "401" "$HTTP_CODE" "Неавторизован" "Неверный статус"

# Слишком длинное сообщение (>500)
LONG_MSG=$(printf 'A%.0s' {1..501})
curl_log "POST" "$BASE_URL/api/suggestions" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN2'" "{\"toProfileId\":\"$PROFILE1_ID\",\"message\":\"$LONG_MSG\"}" "$TMP_DIR/neg_sugg_long.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_sugg_long.txt")
check_test "Предложение с длинным сообщением (400)" "400" "$HTTP_CODE" "Ошибка валидации" "Неверный статус"

# Обновление статуса не получателем
curl_log "PATCH" "$BASE_URL/api/suggestions/$SUGG_ID/status" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN2'" '{"status":"Accepted"}' "$TMP_DIR/neg_sugg_not_receiver.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_sugg_not_receiver.txt")
check_test "Принятие предложения не получателем (403)" "403" "$HTTP_CODE" "Доступ запрещён" "Неверный статус"

# Неверный статус
curl_log "PATCH" "$BASE_URL/api/suggestions/$SUGG_ID/status" "-H 'Content-Type: application/json' -H 'Authorization: Bearer $TOKEN1'" '{"status":"Invalid"}' "$TMP_DIR/neg_sugg_bad_status.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_sugg_bad_status.txt")
check_test "Обновление статуса на неверный (400)" "400" "$HTTP_CODE" "Ошибка валидации" "Неверный статус"

# --- Уведомления ---
curl_log "PATCH" "$BASE_URL/api/notifications/00000000-0000-0000-0000-000000000000/read" "" "" "$TMP_DIR/neg_notif_noauth.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_notif_noauth.txt")
check_test "MarkAsRead без авторизации (401)" "401" "$HTTP_CODE" "Неавторизован" "Неверный статус"

curl_log "POST" "$BASE_URL/api/notifications/read-all" "" "" "$TMP_DIR/neg_notif_readall_noauth.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_notif_readall_noauth.txt")
check_test "MarkAllRead без авторизации (401)" "401" "$HTTP_CODE" "Неавторизован" "Неверный статус"

curl_log "PATCH" "$BASE_URL/api/notifications/settings" "-H 'Content-Type: application/json'" '{"notifyByEmail":false}' "$TMP_DIR/neg_notif_settings_noauth.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_notif_settings_noauth.txt")
check_test "Обновление настроек без авторизации (401)" "401" "$HTTP_CODE" "Неавторизован" "Неверный статус"

# --- Пользователь ---
curl_log "GET" "$BASE_URL/api/user" "" "" "$TMP_DIR/neg_user_noauth.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/neg_user_noauth.txt")
check_test "GET /api/user без авторизации (401)" "401" "$HTTP_CODE" "Неавторизован" "Неверный статус"

# ---------- Удаление профиля User3 ----------
log "=== Удаление профиля User3 ==="
log "Ожидание перед удалением профиля (10 секунд)..."
sleep 10

curl_log "DELETE" "$BASE_URL/api/profiles/me" "-H 'Authorization: Bearer $TOKEN3'" "" "$TMP_DIR/del_profile.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/del_profile.txt")
check_test "DELETE /api/profiles/me (204)" "204" "$HTTP_CODE" "Профиль удалён" "Ошибка удаления"

curl_log "GET" "$BASE_URL/api/profiles/$PROFILE3_ID" "" "" "$TMP_DIR/del_check.txt"
HTTP_CODE=$(get_http_code "$TMP_DIR/del_check.txt")
check_test "GET удалённого профиля (404)" "404" "$HTTP_CODE" "Профиль не найден" "Доступен после удаления"

# ---------- Итоги ----------
log "=== Итоги тестирования ==="
success_count=$PASSED_TESTS
fail_count=${#FAILED_TESTS[@]}
log "Пройдено: $success_count"
log "Провалено: $fail_count"
if [ $fail_count -eq 0 ]; then
  log "Все тесты пройдены успешно."
else
  log "Список непройденных проверок:"
  for t in "${FAILED_TESTS[@]}"; do
    log " - $t"
  done
fi

log "=== Полный тест завершён ==="
echo "Лог сохранён в $LOG_FILE"