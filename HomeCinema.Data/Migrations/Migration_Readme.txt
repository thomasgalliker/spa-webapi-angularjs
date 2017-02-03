update-database -verbose -targetmigration:0 -force -project HomeCinema.Data
add-migration initial -force -project HomeCinema.Data
update-database -verbose -project HomeCinema.Data



