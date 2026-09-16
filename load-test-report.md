# Load Test Results

Generated: 2026-09-16 15:43:51 UTC
Target: https://m-zhehistovskyi-web-app-ckf7bscegsdkewhn.westeurope-01.azurewebsites.net
Run marker: LOADTEST_20260916154332_1718
Total requests: 300
Max concurrency: 20
Target rate: 20,0 req/s (jittered)

## Как запускать

1. Поднять API и БД из корня решения: `docker compose up -d`.
2. Дождаться, пока API станет доступен (https://m-zhehistovskyi-web-app-ckf7bscegsdkewhn.westeurope-01.azurewebsites.net).
3. Запустить нагрузочный тест:

   ```
   dotnet run --project ConferenceRoomBooking.LoadTesting -- --requests 300 --max-concurrency 20 --rate 20 --base-url https://m-zhehistovskyi-web-app-ckf7bscegsdkewhn.westeurope-01.azurewebsites.net
   ```

4. Каждый запуск случайно перемешивает все доступные сценарии (GET/POST/PUT/DELETE) вместо
   последовательных пакетов, и не более `--max-concurrency` запросов выполняется одновременно.
5. Все созданные тестовые данные помечены префиксом run marker — см. раздел очистки ниже.
   Этот файл перезаписывается при каждом запуске.

## Overall

| Total | Max concurrency | Avg concurrency | Total time (s) | Avg (ms) | Min (ms) | Max (ms) | Success | Failed |
|---|---|---|---|---|---|---|---|---|
| 300 | 20 | 3,8 | 17,67 | 226,3 | 143,5 | 1335,6 | 300 | 0 |

## By scenario

| Scenario | Count | Success | Failed | Avg (ms) | Min (ms) | Max (ms) |
|---|---|---|---|---|---|---|
| GET /api/rooms/available | 49 | 49 | 0 | 184,5 | 159,6 | 785,9 |
| POST /api/bookings | 35 | 35 | 0 | 218,6 | 206,7 | 303,5 |
| GET /api/ServiceOptions | 32 | 32 | 0 | 149,8 | 144,6 | 176,6 |
| GET /api/rooms/{roomId} | 27 | 27 | 0 | 148,9 | 143,6 | 188,2 |
| GET /api/bookings/my | 26 | 26 | 0 | 437,9 | 180,8 | 1335,6 |
| GET /api/bookings/{bookingId} | 22 | 22 | 0 | 150,4 | 143,5 | 195,2 |
| GET /api/ServiceOptions/{serviceOptionId} | 19 | 19 | 0 | 232,7 | 143,9 | 971,4 |
| POST /api/auth/login | 17 | 17 | 0 | 219,9 | 165,8 | 747,1 |
| DELETE /api/ServiceOptions/{serviceOptionId} | 14 | 14 | 0 | 396,0 | 386,5 | 419,9 |
| GET /api/analytics/service-performance | 11 | 11 | 0 | 169,8 | 147,0 | 373,6 |
| POST /api/auth/register | 11 | 11 | 0 | 205,9 | 200,0 | 220,8 |
| PUT /api/rooms/{roomId} | 10 | 10 | 0 | 271,7 | 238,4 | 462,6 |
| POST /api/ServiceOptions | 8 | 8 | 0 | 210,1 | 179,1 | 338,4 |
| DELETE /api/rooms/{roomId} | 7 | 7 | 0 | 370,6 | 359,2 | 385,8 |
| PUT /api/ServiceOptions/{serviceOptionId} | 5 | 5 | 0 | 359,5 | 350,2 | 368,6 |
| GET /api/analytics/room-performance | 4 | 4 | 0 | 152,7 | 148,7 | 160,2 |
| POST /api/rooms | 3 | 3 | 0 | 152,7 | 151,7 | 154,0 |

## Test data created (needs manual cleanup)

Run marker: `LOADTEST_20260916154332_1718`. Everything this run created is tagged — find it all via `Name LIKE 'LOADTEST_20260916154332_1718%'`.

| Room id | Room name |
|---|---|
| 81eb7060-e5b1-f111-a6a7-6045bde9b4a7 | LOADTEST_20260916154332_1718_Room_1 |
| 82eb7060-e5b1-f111-a6a7-6045bde9b4a7 | LOADTEST_20260916154332_1718_Room_2 |
| 83eb7060-e5b1-f111-a6a7-6045bde9b4a7 | LOADTEST_20260916154332_1718_Room_3 |

| Service option id |
|---|
| 7feb7060-e5b1-f111-a6a7-6045bde9b4a7 |
| 80eb7060-e5b1-f111-a6a7-6045bde9b4a7 |

| Registered user email |
|---|
| loadtest.loadtest_20260916154332_1718.7@conference-room-booking.local |
| loadtest.loadtest_20260916154332_1718.11@conference-room-booking.local |
| loadtest.loadtest_20260916154332_1718.10@conference-room-booking.local |
| loadtest.loadtest_20260916154332_1718.9@conference-room-booking.local |
| loadtest.loadtest_20260916154332_1718.8@conference-room-booking.local |
| loadtest.loadtest_20260916154332_1718.6@conference-room-booking.local |
| loadtest.loadtest_20260916154332_1718.5@conference-room-booking.local |
| loadtest.loadtest_20260916154332_1718.4@conference-room-booking.local |
| loadtest.loadtest_20260916154332_1718.3@conference-room-booking.local |
| loadtest.loadtest_20260916154332_1718.2@conference-room-booking.local |
| loadtest.loadtest_20260916154332_1718.1@conference-room-booking.local |

Bookings created this run: 35. Bookings have no delete/cancel API
endpoint — they are only reachable by joining `Bookings.RoomId` against the fixture room ids above,
so a direct DB query is the only way to purge them.

Note: `CreateRoomScenario`/`CreateServiceOptionScenario` dispatches also leave extra tagged, undeleted
entities named `LOADTEST_20260916154332_1718_Room_Extra*` / `LOADTEST_20260916154332_1718_Service_Extra*` not listed above — the
same `Name LIKE` query covers them.
