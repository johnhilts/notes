# grep all config files containing Newtonsoft.Json, pipe to awk to get filename, then remove trailing ':' then piple list of files back to grep to find a different string
grep -r 'Newtonsoft.Json' --include='*.config' --exclude-dir='node_module' --exclude-dir='Dependencies' | awk '{print $1}' | tr -d ':' | xargs grep 'bindingRedirect'
