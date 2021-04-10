#!/module/for/bash
rabbitmqctl set_policy ha-fed \
    ".*" '{"federation-upstream-set":"all", "ha-sync-mode":"automatic", "ha-mode":"nodes", "ha-params":["rabbit@rabbit-1","rabbit@rabbit-2","rabbit@rabbit-3"]}' \
    --priority 1 \
    --apply-to queues