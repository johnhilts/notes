grep -irl 'TestResultCopy' --exclude='*.jar' --exclude='*.class'
grep -irl 'TestResultCopy' --exclude='*.jar' --exclude='*.class' --exclude='*.lock' --exclude='*.java'
grep -irl 'TestResultCopy.TestResultCopyTaskConfigurator' --exclude='*.jar' --exclude='*.class' --exclude='*.lock'
grep -irl 'buildKey' --exclude='*.jar' --exclude='*.class' --exclude='*.lock'
grep -irl 'artifactName' --exclude='*.jar' --exclude='*.class' --exclude='*.lock'
grep -irl 'artifactName' --exclude='*.jar' --exclude='*.class' --exclude='*.lock' | vim -
grep -irl 'current_session_context_class' --exclude='*.jar' --exclude='*.class' --exclude='*.lock'
grep -irl 'serverRootUrl' --exclude='*.jar' --exclude='*.class' --exclude='*.lock'
grep -irl 'buildKey' --exclude='*.jar' --exclude='*.class' --exclude='*.lock' 
grep -irl 'buildKey' --exclude='*.jar' --exclude='*.class' --exclude='*.lock' --exclude-dir=target
grep -irl 'atlassian-plugin key' --exclude='*.jar' --exclude='*.class' --exclude='*.lock' --exclude-dir=target
grep -ir 'atlassian-plugin key' --exclude='*.jar' --exclude='*.class' --exclude='*.lock' --exclude-dir=target
grep -Fr 1.0.3  --exclude='*.jar' --exclude='*.class' --exclude='*.lock' --exclude-dir=target
grep -Fr 1.0.3  --exclude='*.jar' --exclude='*.class' --exclude='*.lock' --exclude-dir=target
grep -r 'BED' --exclude='*.jar' --exclude='*.class' --exclude='*.lock' --exclude-dir=target --include='*.java'
