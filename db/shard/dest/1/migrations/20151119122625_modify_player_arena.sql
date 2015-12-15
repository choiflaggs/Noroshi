
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `player_arena`
 CHANGE `DeckCharacter1` `DeckPlayerCharacterID1` int unsigned NOT NULL,
 CHANGE `DeckCharacter2` `DeckPlayerCharacterID2` int unsigned NOT NULL,
 CHANGE `DeckCharacter3` `DeckPlayerCharacterID3` int unsigned NOT NULL,
 CHANGE `DeckCharacter4` `DeckPlayerCharacterID4` int unsigned NOT NULL,
 CHANGE `DeckCharacter5` `DeckPlayerCharacterID5` int unsigned NOT NULL,
 CHANGE `PlayCount` `PlayNum` int unsigned,
 CHANGE `ResetCount` `ResetNum` int unsigned,
 CHANGE `CoolTime` `CoolTimeAt` int unsigned,
 ADD `BestRank` int unsigned NOT NULL AFTER `Rank`,
 ADD `BattleStartedAt`  int unsigned NOT NULL AFTER `ResetNum`,
 ADD KEY `i1` (`Rank`,`BattleStartedAt`);

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `player_arena`
 DROP KEY `i1`,
 DROP `BattleStartedAt`,
 DROP `BestRank`,
 CHANGE `CoolTimeAt` `CoolTime` int,
 CHANGE `ResetNum` `ResetCount` int,
 CHANGE `PlayNum` `PlayCount` int,
 CHANGE `DeckPlayerCharacterID5` `DeckCharacter5` int,
 CHANGE `DeckPlayerCharacterID4` `DeckCharacter4` int,
 CHANGE `DeckPlayerCharacterID3` `DeckCharacter3` int,
 CHANGE `DeckPlayerCharacterID2` `DeckCharacter2` int,
 CHANGE `DeckPlayerCharacterID1` `DeckCharacter1` int;
