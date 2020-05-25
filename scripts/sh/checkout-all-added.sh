  # the -n1 in xargs is needed so that it feeds 1 line at a time to the shell script
  # actual shell commands probably just parse on the \n to split the lines into mulitple args
  git status | grep 'modified' | awk '{print $2}' | xargs -n1 ./checkoutfile.sh 
