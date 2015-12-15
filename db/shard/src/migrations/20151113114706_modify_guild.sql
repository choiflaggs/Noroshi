
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `player_status`
  CHANGE COLUMN `Language` `LanguageID` int unsigned NOT NULL,
  DROP KEY `i1`,
  ADD KEY `i1` (`GuildID`,`UpdatedAt`);

ALTER TABLE `guild`
  ADD COLUMN `MemberAndRequestNumCluster` tinyint unsigned NOT NULL AFTER `RequestNum`,
  ADD COLUMN `AveragePlayerLevel` smallint unsigned NOT NULL AFTER `MemberAndRequestNumCluster`,
  CHANGE COLUMN `OwnerPlayerID` `LeaderPlayerID` int unsigned NOT NULL,
  DROP KEY `i1`,
  ADD KEY `i1` (`Category`,`MemberAndRequestNumCluster`,`AveragePlayerLevel`),
  ADD KEY `i2` (`Category`,`AveragePlayerLevel`);

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `player_status`
  CHANGE COLUMN `LanguageID` `Language` int unsigned NOT NULL,
  DROP KEY `i1`,
  ADD KEY `i1` (`GuildID`);

ALTER TABLE `guild`
  DROP COLUMN `MemberAndRequestNumCluster`,
  DROP COLUMN `AveragePlayerLevel`,
  CHANGE COLUMN `LeaderPlayerID` `OwnerPlayerID` int unsigned NOT NULL,
  DROP KEY `i1`,
  ADD KEY `i1` (`Category`,`MemberNum`),
  DROP KEY `i2`;

