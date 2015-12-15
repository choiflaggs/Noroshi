-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

CREATE TABLE `sequential_id_guild_chat_message` (
  `ID` int unsigned NOT NULL
) ENGINE=MyISAM;
INSERT INTO sequential_id_guild_chat_message VALUES (0);

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `sequential_id_guild_chat_message`;

