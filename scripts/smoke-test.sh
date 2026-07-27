#!/usr/bin/env bash
# Quick end-to-end smoke test of the live Outty API, meant to be run right before
# recording the demo/presentation to confirm everything actually works.
#
# Usage: ./scripts/smoke-test.sh
#
# Uses two of the dummy test profiles already seeded in the database:
#   Profile 2 = Alex Rivera (Atlanta, GA)
#   Profile 3 = Jamie Chen  (Atlanta, GA)
# Both share the "Hiking" interest and live in the same state, so they're
# guaranteed to match each other.

set -euo pipefail

BASE_URL="https://outty-api.azurewebsites.net"
PROFILE_A=2   # Alex Rivera
PROFILE_B=3   # Jamie Chen
USER_A=2
USER_B=3

pass() { echo "  PASS - $1"; }
fail() { echo "  FAIL - $1"; exit 1; }

echo "=== 1. API is reachable ==="
STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL/states")
[ "$STATUS" = "200" ] && pass "GET /states -> 200" || fail "GET /states -> $STATUS"

echo "=== 2. States list is populated ==="
COUNT=$(curl -s "$BASE_URL/states" | python3 -c "import json,sys; print(len(json.load(sys.stdin)))")
[ "$COUNT" = "51" ] && pass "51 states returned" || fail "expected 51 states, got $COUNT"

echo "=== 3. Match candidates (Alex should see Jamie or similar) ==="
CANDIDATES=$(curl -s "$BASE_URL/matches/candidates/$PROFILE_A")
echo "  $CANDIDATES" | python3 -m json.tool | sed 's/^/  /'
[ "$CANDIDATES" != "[]" ] && pass "candidates list is non-empty" || fail "no candidates returned - check seed data"

echo "=== 4. Record a mutual swipe (like) to create a match ==="
SWIPE_A=$(curl -s -X POST "$BASE_URL/matches/swipe" \
  -H "Content-Type: application/json" \
  -d "{\"swiperProfileId\":$PROFILE_A,\"targetProfileId\":$PROFILE_B,\"liked\":true}")
echo "  A likes B: $SWIPE_A"

SWIPE_B=$(curl -s -X POST "$BASE_URL/matches/swipe" \
  -H "Content-Type: application/json" \
  -d "{\"swiperProfileId\":$PROFILE_B,\"targetProfileId\":$PROFILE_A,\"liked\":true}")
echo "  B likes A: $SWIPE_B"

IS_MATCH=$(echo "$SWIPE_B" | python3 -c "import json,sys; print(json.load(sys.stdin)['isMatch'])")
[ "$IS_MATCH" = "True" ] && pass "mutual like registered as a match" || fail "expected isMatch=true, got $IS_MATCH"

echo "=== 5. Match shows up in GET /matches ==="
MATCHES_A=$(curl -s "$BASE_URL/matches/$PROFILE_A")
echo "  $MATCHES_A" | python3 -m json.tool | sed 's/^/  /'
[ "$MATCHES_A" != "[]" ] && pass "match appears in Alex's match list" || fail "match list is empty"

echo "=== 6. Create/find a conversation for the match ==="
CONVO_ID=$(curl -s -X POST "$BASE_URL/api/Messaging/conversations" \
  -H "Content-Type: application/json" \
  -d "{\"currentUserId\":$USER_A,\"otherUserId\":$USER_B}")
echo "  Conversation ID: $CONVO_ID"
[ -n "$CONVO_ID" ] && pass "conversation created/found" || fail "no conversation ID returned"

echo "=== 7. Send a message ==="
SEND_RESULT=$(curl -s -X POST "$BASE_URL/api/Messaging/conversations/$CONVO_ID/messages" \
  -H "Content-Type: application/json" \
  -d "{\"senderId\":$USER_A,\"content\":\"Smoke test message - ignore\"}")
echo "  $SEND_RESULT"
echo "$SEND_RESULT" | python3 -c "import json,sys; json.load(sys.stdin)['id']" >/dev/null 2>&1 \
  && pass "message sent" || fail "message send failed"

echo "=== 8. Message shows up when fetched back ==="
MESSAGES=$(curl -s "$BASE_URL/api/Messaging/conversations/$CONVO_ID/messages?currentUserId=$USER_A")
echo "$MESSAGES" | python3 -c "import json,sys; d=json.load(sys.stdin); assert len(d) > 0" \
  && pass "message list is non-empty" || fail "no messages returned"

echo
echo "All checks passed. The live API is healthy for the demo."
