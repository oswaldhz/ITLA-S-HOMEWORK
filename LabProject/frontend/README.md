# LabProject Frontend

The frontend targets the API at `https://localhost:7260/api` by default, matching the HTTPS development profile in
`Properties/launchSettings.json`. If you run the backend on the HTTP profile (`http://localhost:5159`) or another host/port,
override the API base URL by setting `VITE_API_URL` (see `.env.example`).
