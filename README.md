# Connections API

An archive API for the New York Times [Connections](https://www.nytimes.com/games/connections) puzzle.
A background job pulls new puzzles from the NYT on a schedule and stores them in SQLite; the API serves
them by date or as a paged list.

## Endpoints

All routes are `GET` and allow any origin.

| Route | Returns |
|---|---|
| `/connections/{printDate}` | The puzzle for a date (`yyyy-MM-dd`), or 404. |
| `/connections/query?page=1&count=20` | A page of puzzles, oldest first. Optional `year` and `month` filter by print date. |

Both take `hideSolutions=true` to leave out which cards belong together.

A puzzle looks like this:

```json
{
  "puzzleNumber": 520,
  "printDate": "2024-11-12",
  "previousPuzzle": "2024-11-11",
  "nextPuzzle": "2024-11-13",
  "editor": "...",
  "categories": [
    {
      "title": "...",
      "color": "yellow",
      "orderingKey": 0,
      "cards": [{ "content": "...", "position": 3 }]
    }
  ]
}
```

The query endpoint wraps puzzles in `{ "page", "count", "maxCount", "data" }`.

## Sync

`SyncScheduler` runs on a cron schedule (hourly by default). Each run works out which dates are missing,
from the first puzzle (or the last synced one) up to today, and fetches them from the NYT in batches.

| Setting | What it does |
|---|---|
| `ConnectionStrings:ConnectionsContext` | SQLite connection string. |
| `Sync:ScheduleCron` | Cron expression for the sync. Invalid or missing means hourly. |
| `Sync:RunImmediately` | Run a sync on startup instead of waiting for the first cron tick. |

Settings can also come from environment variables, e.g. `Sync__ScheduleCron`.

## Running it

Needs the .NET 10 SDK.

```sh
dotnet run
```

Development uses `.tmp/dev.db` and syncs on startup. Outside development the app applies EF Core
migrations when it starts.

### Docker

The `Image` workflow publishes `ghcr.io/mtfarkas/connectionsapi` on every push to `master`.

```sh
docker run -p 8080:8080 \
  -e ConnectionStrings__ConnectionsContext="Data Source=/data/connections.db" \
  -v connections-data:/data \
  ghcr.io/mtfarkas/connectionsapi:latest
```

In a non-development container the app expects to sit behind a reverse proxy: it honours
`X-Forwarded-*` headers and serves everything under `/connections-api`.

## License

MIT, see [LICENSE](LICENSE).
