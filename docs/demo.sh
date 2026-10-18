#!/usr/bin/env bash

set -euo pipefail

CATALOG=${CATALOG:-http://localhost:5001}
ORDERS=${ORDERS:-http://localhost:5002}
PAYMENTS=${PAYMENTS:-http://localhost:5003}
NOTIFICATIONS=${NOTIFICATIONS:-http://localhost:5004}

json_field() { grep -oE "\"$1\":\"[^\"]*\"" | head -1 | cut -d'"' -f4; }

step() { printf '\n\033[1;34m== %s\033[0m\n' "$1"; }

step "1. Create a product (Catalog -> MongoDB)"
PRODUCT=$(curl -sf -X POST "$CATALOG/catalog/products" \
  -H 'Content-Type: application/json' \
  -d '{"name":"Smart Desk Lamp","description":"LED desk lamp with adjustable colour temperature and USB-C charging port.","category":"Home","price":49.90,"stock":25}')
echo "$PRODUCT"
PRODUCT_ID=$(echo "$PRODUCT" | json_field id)

step "2. Place an order (Orders -> Catalog check -> Redis -> publish OrderPlaced)"
ORDER=$(curl -sf -X POST "$ORDERS/orders" \
  -H 'Content-Type: application/json' \
  -d "{\"productId\":\"$PRODUCT_ID\",\"quantity\":2}")
echo "$ORDER"
ORDER_ID=$(echo "$ORDER" | json_field id)

step "3. Wait for the payment events (OrderPlaced -> Payments -> PaymentProcessed -> Orders)"
for _ in $(seq 1 20); do
  STATUS=$(curl -sf "$ORDERS/orders/$ORDER_ID" | json_field status)
  echo "order status: $STATUS"
  [ "$STATUS" = "Paid" ] && break
  sleep 0.5
done
curl -sf "$ORDERS/orders/$ORDER_ID"; echo

step "4. Payment record (Payments -> PostgreSQL)"
curl -sf "$PAYMENTS/payments/$ORDER_ID"; echo

step "5. Notification (Notifications -> SQL Server)"
for _ in $(seq 1 20); do
  NOTES=$(curl -sf "$NOTIFICATIONS/notifications?orderId=$ORDER_ID")
  [ "$NOTES" != "[]" ] && break
  sleep 0.5
done
echo "$NOTES"

step "6. Ask the AI assistant (Catalog -> LLM with the catalog as context)"
curl -s -w '\nHTTP %{http_code}\n' -X POST "$CATALOG/catalog/assistant" \
  -H 'Content-Type: application/json' \
  -d '{"question":"I need something for my desk and something for working out. What do you have and how much is it?"}'

step "Done. Demo product id: $PRODUCT_ID, order id: $ORDER_ID"
