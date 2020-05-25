  # this uses regex groups - the key is that you have to escape the parentheses
  # this one has 3 groups, and replaces "me" with "YOU" and groups the surrounding conditions so only the 2nd group actually gets altered
  sed 's/\(\s\)\(me\)\(\W\)/\1YOU\3/g' testfile.txt 
