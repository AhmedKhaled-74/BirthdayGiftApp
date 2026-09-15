# Free hosting on Render with Neon Postgres

The hosted app runs as one Render Free Web Service (Docker, ASP.NET Core serving the Angular `wwwroot` build) backed by Neon Free Postgres over `Npgsql`, with Postgres also used locally so there is a single EF provider and migration set.

Render Web Free needs no credit card and sleeps instead of billing when over quota; Neon Free has no expiry (unlike Render Postgres, deleted after 30 days) and wakes in milliseconds (unlike Supabase, which pauses after 7 days idle and needs manual restore). The trade-off is cold starts (~60s web + sub-second DB wake), 512MB/0.1 CPU, 0.5GB DB, and 100 CU-hours/month, accepted as fine for this demo app.
