# Catera Leaflet Map Setup

This project uses Leaflet.js with OpenStreetMap tiles for in-app restaurant/caterer discovery.

No Google Maps, Google Places, Google Cloud project, API key, or billing setup is required.

## Libraries

The map views load Leaflet from CDN:

- `https://unpkg.com/leaflet@1.9.4/dist/leaflet.css`
- `https://unpkg.com/leaflet@1.9.4/dist/leaflet.js`

Tiles are loaded from OpenStreetMap:

- `https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png`

Attribution is included in the Leaflet tile layer:

- `© OpenStreetMap contributors`

## Data Source

Map markers come from the local SQL Server database, not a paid external Places API.

Endpoint:

```text
GET /Nearby/Locations
```

The endpoint returns approved caterers with valid coordinates:

```json
{
  "id": 1,
  "name": "Istanbul Garden Catering",
  "address": "Besiktas, Istanbul",
  "category": "Corporate Lunch",
  "latitude": 41.043,
  "longitude": 29.0094
}
```

## Run

```powershell
dotnet ef database update --project Northwind.Mvc --context CateringProjectContext
dotnet run --project Northwind.Mvc --urls http://localhost:5254
```

Open:

- User nearby discovery: `http://localhost:5254/Nearby`
- Admin location management: `http://localhost:5254/Admin/LinkRestaurants`

## Admin Location Editing

On `Admin/LinkRestaurants`:

1. Select a caterer from the right-side list.
2. Click a new point on the Leaflet map.
3. Press `Save Location`.

The coordinates are saved through:

```text
POST /Nearby/UpdateCatererLocation
```
