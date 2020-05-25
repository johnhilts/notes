file="$1"
tr -d '\r' < "$file" > "$file"2
mv "$file"2 "$file"
