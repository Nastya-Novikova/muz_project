#!/bin/bash
set +e

LOG_FILE="test_favorites_log.txt"
BASE_URL="http://localhost:7000"
VERIFICATION_CODE="111111"
TMP_DIR=$(mktemp -d)

# Проверка наличия jq
JQ=$(command -v jq 2>/dev/null)
if [ -z "$JQ" ]; then
    # Попробуем найти в стандартных местах установки в Git Bash
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

log "=== Запуск теста добавления в избранное ==="
log "Временная папка: $TMP_DIR"

# 1. Пересборка и запуск
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

  # Запрос кода
  curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/auth/code" \
    -H "Content-Type: application/json" \
    -d "{\"email\":\"$EMAIL\"}" > "$TMP_DIR/code_$EMAIL.txt"
  dump_response "$TMP_DIR/code_$EMAIL.txt" "  Код подтверждения"

  # Вход
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

  # Создание профиля
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

# Регистрация двух пользователей
log "=== Регистрация первого пользователя ==="
USER1_DATA=$(register_user "user1@test.com" "User One")
read TOKEN1 USER1_ID PROFILE1_ID <<< "$USER1_DATA"
log "Первый пользователь: userId=$USER1_ID, profileId=$PROFILE1_ID"

log "=== Регистрация второго пользователя ==="
USER2_DATA=$(register_user "user2@test.com" "User Two")
read TOKEN2 USER2_ID PROFILE2_ID <<< "$USER2_DATA"
log "Второй пользователь: userId=$USER2_ID, profileId=$PROFILE2_ID"

# Добавление в избранное
log "=== Добавление первого профиля в избранное второго ==="
IDEMPOTENCY_KEY=$(date +%s%N)
curl -s -w "\n%{http_code}" -X PUT "$BASE_URL/api/$PROFILE1_ID/favorite" \
  -H "Authorization: Bearer $TOKEN2" \
  -H "Idempotency-Key: $IDEMPOTENCY_KEY" > "$TMP_DIR/favorite.txt"
dump_response "$TMP_DIR/favorite.txt" "Ответ операции"
HTTP_CODE=$(tail -1 "$TMP_DIR/favorite.txt")
log "HTTP статус: $HTTP_CODE"

if [ "$HTTP_CODE" = "204" ] || [ "$HTTP_CODE" = "200" ]; then
  log "УСПЕХ: добавление в избранное выполнено"
else
  log "ОШИБКА: неожиданный статус $HTTP_CODE"
fi

# Проверка флага isFavorite
log "=== Проверка флага isFavorite ==="
curl -s -w "\n%{http_code}" -X GET "$BASE_URL/api/profiles/$PROFILE1_ID" \
  -H "Authorization: Bearer $TOKEN2" > "$TMP_DIR/get_profile.txt"
dump_response "$TMP_DIR/get_profile.txt" "Получение профиля"
PROFILE_BODY=$(sed '$d' "$TMP_DIR/get_profile.txt")
IS_FAV=$("$JQ" -r '.isFavorite' <<< "$PROFILE_BODY")
log "isFavorite = $IS_FAV"

if [ "$IS_FAV" = "true" ]; then
  log "УСПЕХ: флаг isFavorite = true"
else
  log "ОШИБКА: ожидалось true, получено $IS_FAV"
fi

log "=== Тест завершён ==="
echo "Лог сохранён в $LOG_FILE"