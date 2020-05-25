# These are the scripts I sent yesterday. They will handle encoding just the SQL files that need it by removing the hex FFFE marker and taking out all of the hex 0s so that double-byte characters are reverted to single-byte. 

# I ran these in bash in the Database folder:
grep -rbl $'\xff' --include=*.sql | sed 's/\(.*\)/"\1"/g' | xargs sed -i 's/\x0//g'
grep -rbl $'\xff' --include=*.sql | sed 's/\(.*\)/"\1"/g' | xargs sed -i '1 s/^\xff\xfe/{migration}/'
# COMMIT HERE
grep -rl '{migration}' --include=*.sql | sed 's/\(.*\)/"\1"/g' | xargs sed -i '1 s/^{migration}/\r\n/'
# COMMIT HERE AGAIN

# First line converts from double to single byte by removing all hex zeroes.
# Second line replaces hex FFFE with a placholder “{migration}”
# Third line replaces the placeholder with a \r\n line break to count as the first text change after converting from binary. After this, any new changes to any SQL file should diff properly in all Atlassian tools.
# 

