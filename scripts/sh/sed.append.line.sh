# the input file would need to be sorted / made unique to prevent duplicates
cat sed.LP-30867.files.txt | xargs sed -i '/using LampsPlus.Commerce.Web.Models.Tags;/a \
using LampsPlus.Commerce.Core.Tags;\
'
