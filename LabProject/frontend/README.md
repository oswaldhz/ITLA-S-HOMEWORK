# LabProject Frontend

The frontend targets the API at `https://localhost:7095/api` by default, matching the backend development profile (`LabProject.Api`). Make sure that backend is running before trying to sign in; otherwise the login form will surface a connection error. Override the API base URL by setting `VITE_API_URL` (see `.env.example`) if your backend runs elsewhere.
