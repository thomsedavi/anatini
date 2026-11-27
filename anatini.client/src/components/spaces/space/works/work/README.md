# Space Work
- Displays space work with correct HTML ✓
- Does not display date if undated ✓
- DateTime attribute of Time includes time if date has time component
- DateTime attribute of Time excludes time if date has no time component
- Displays year if date is year ✓
- Displays month/year if date is month/year ✓
- Displays day/month/year if date is day/month/year ✓
- Displays hour/day/month/year if date is hour/day/month/year ✓
- Displays minute/hour/day/month/year if date is minute/hour/day/month/year ✓
- Does not display owner in article header ✓
- Links back to space work list ✓
- Can star, bookmark, and dismiss when logged in ✓
- Cannot star, bookmark, or dismiss when not logged in ✓
- Has link to open edit page when owner ✓
- Does not have link to open edit page when:
  - not logged in ✓
  - logged in as other user ✓
- Can view protected work when:
  - logged in as owner ✓
  - logged in as trusted other user ✓
- Cannot view protected work when:
  - not logged in ✓
  - logged in as untrusted other user ✓
- Can view public work when:
  - not logged in ✓
  - logged in as owner ✓
  - logged in as other user ✓
- Can view draft work when logged in as owner ✓
- Cannot view draft work when:
  - not logged in ✓
  - logged in as other user ✓
- Can view archived work when logged in as owner ✓
- Cannot view archived work when:
  - not logged in ✓
  - logged in as other user ✓
- Can view future work when logged in as owner ✓
- Cannot view future work when:
  - not logged in ✓
  - logged in as other user ✓
- Displays 404 page if not found ✓
- Displays 500 page if internal server error ✓
- Draft/Archived/Scheduled is visually distinct ✓
- Protected notes should show protected status ✓
# Space Work Create
# Space Work Edit

https://localhost:58343/spaces/my-space/works/public-published
https://localhost:58343/spaces/my-space/works/protected-published
https://localhost:58343/spaces/my-space/works/public-draft
https://localhost:58343/spaces/my-space/works/public-archived
https://localhost:58343/spaces/my-space/works/public-scheduled
