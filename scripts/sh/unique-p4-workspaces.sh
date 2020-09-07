# Created with this in bash:
awk '/2018/{print $5}' p4workspaces.txt | sort | uniq -i > p4workspaces.2018.distinct.txt

# The p4workspaces.txt file was generated with:
p4 workspaces > p4workspaces.txt

# Each workspace is unique, but there is still overlap. I think the list can still help us get an idea of what was worked on this year, though …

