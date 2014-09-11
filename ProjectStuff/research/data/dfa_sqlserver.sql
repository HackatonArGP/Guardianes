-- Barend Gehrels, March 2011

CREATE TABLE htr_adopters (
    gid integer primary key,
    id integer,
    cellnum integer,
    transaction_nr integer,
    name varchar(50),
    username varchar(50),
    amount integer,
    area integer,
    geom geometry
);

CREATE TABLE htr_kphp (
	gid integer primary key,
	geom geometry
);




-- For import data:
-- 1: go to PostGreSQL and go to the database
-- 2: execute next two statements
-- Selecteer in PostGreSQL:
-- select 'insert into htr_adopters(gid,transaction_nr,name,username,amount,area,geom) values(' || cast(gid as text)||','||cast(transaction as text) ||',\''||name||'\',\''||username||'\','||amount||','||area||',geometry::STGeomFromText(\''||ST_AsText(geom)||'\',900913));' from htr_adopters
-- select 'insert into htr_kphp(gid,geom) values(' || cast(gid as text)||','||',geometry::STGeomFromText(\''||ST_AsText(geom)||'\',900913));' from htr_kphp_900913

-- 3: copy the results into the clipboard (remove enclosing double quotes)
-- 4: paste them in SQL Server Management Studio
-- 5: done
-- NOTE: this is not the COMPLETE conversion (can be done like this) but just some small essential things

