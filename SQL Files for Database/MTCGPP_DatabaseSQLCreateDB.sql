create table player
(
    id        serial
        constraint player_pk
            primary key,
    username  varchar not null,
    password  varchar not null,
    authtoken varchar not null,
    coins     integer not null,
    elo       integer not null,
    games     integer not null,
    wins      integer not null
);

alter table player
    owner to postgres;

create unique index player_id_uindex
    on player (id);

create table cards
(
    cardid      serial
        constraint cards_pk
            primary key,
    name        varchar not null,
    damage      integer not null,
    cardtype    varchar not null,
    element     varchar not null,
    monstertype varchar not null
);

alter table cards
    owner to postgres;

create unique index cards_cardid_uindex
    on cards (cardid);

create table playerstack
(
    playerid integer                                                       not null
        constraint playercards_player_id_fk
            references player
            on delete cascade,
    cardid   integer                                                       not null
        constraint playercards_cards_cardid_fk
            references cards
            on delete cascade,
    number	serial
        constraint trades_pk
            primary key,
);

alter table playerstack
    owner to postgres;

create unique index playerstack_number_uindex
    on playerstack (number);

create table playerdeck
(
    playerid integer not null
        constraint playerdeck_player_id_fk
            references player
            on delete cascade,
    cardid   integer not null
        constraint playerdeck_cards_cardid_fk
            references cards
            on delete cascade
);

alter table playerdeck
    owner to postgres;

create table trades
(
    tradeid        serial
        constraint trades_pk
            primary key,
    ownerid        integer not null
        constraint trades_player_id_fk
            references player
            on delete cascade,
    targetcardtype varchar,
    mindmg         integer,
    coinprice      integer,
    tradedcardid   integer not null
        constraint trades_cards_cardid_fk
            references cards
            on delete cascade
);

alter table trades
    owner to postgres;

create unique index trades_tradeid_uindex
    on trades (tradeid);


