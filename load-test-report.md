# Load Test Results

Generated: 2026-09-04 10:07:10 UTC
Target: http://localhost:5000
Requests per run: 1000
Parallelism levels: 10, 50, 100

## Как запускать

1. Поднять API и БД из корня решения: `docker compose up -d`.
2. Дождаться, пока API станет доступен (http://localhost:5000).
3. Запустить нагрузочный тест:

   ```
   dotnet run --project ConferenceRoomBooking.LoadTesting -- --requests 1000 --parallelism-levels 10,50,100 --base-url http://localhost:5000
   ```

4. Результат — таблицы ниже, этот файл перезаписывается при каждом запуске.

## GET /api/rooms/available

| Parallelism | Total time (s) | Avg (ms) | Min (ms) | Max (ms) | Avg concurrency | Success | Failed |
|---|---|---|---|---|---|---|---|
| 10 | 15,07 | 150,4 | 109,4 | 6783,3 | 10,0 | 1000 | 0 |
| 50 | 9,44 | 416,4 | 109,1 | 9441,9 | 44,1 | 1000 | 0 |
| 100 | 6,65 | 536,5 | 108,8 | 6642,5 | 80,7 | 1000 | 0 |

## POST /api/bookings

| Parallelism | Total time (s) | Avg (ms) | Min (ms) | Max (ms) | Avg concurrency | Success | Failed |
|---|---|---|---|---|---|---|---|
| 10 | 121,06 | 1205,2 | 344,7 | 1917,7 | 10,0 | 1000 | 0 |
| 50 | 122,23 | 5962,2 | 350,0 | 7548,0 | 48,8 | 1000 | 0 |
| 100 | 124,13 | 11808,9 | 349,6 | 14901,0 | 95,1 | 1000 | 0 |

## PUT /api/rooms/{roomId}

| Parallelism | Total time (s) | Avg (ms) | Min (ms) | Max (ms) | Avg concurrency | Success | Failed |
|---|---|---|---|---|---|---|---|
| 10 | 34,71 | 345,9 | 335,4 | 662,0 | 10,0 | 1000 | 0 |
| 50 | 10,49 | 510,9 | 337,1 | 3210,7 | 48,7 | 1000 | 0 |
| 100 | 12,31 | 1171,7 | 336,5 | 12302,7 | 95,2 | 1000 | 0 |

