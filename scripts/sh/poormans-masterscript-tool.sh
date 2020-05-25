git log feature/backoffice-and-admin ^master --no-merges --name-only | grep -i '\.sql' | sort -u | xargs cat > masterscript.sql
