-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
CREATE TABLE `guild_chat_message` (
 `ID` int unsigned NOT NULL,
 `GuildID` int unsigned NOT NULL,
 `Status` tinyint unsigned NOT NULL,
 `PlayerID` int unsigned NOT NULL,
 `Message` text NOT NULL,
 `CreatedAt` int unsigned NOT NULL,
 PRIMARY KEY (`ID`,`CreatedAt`),
 KEY `i1` (`GuildID`,`Status`,`CreatedAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
DROP TABLE `guild_chat_message`;

