
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `action`
  CHANGE COLUMN `AttributeID` `AttributeID1` int unsigned NOT NULL,
  CHANGE COLUMN `AttributeIntercept` `AttributeIntercept1` float NOT NULL,
  CHANGE COLUMN `AttributeSlope` `AttributeSlope1` float NOT NULL,
  ADD COLUMN `AttributeID2` int unsigned NOT NULL AFTER `AttributeSlope1`,
  ADD COLUMN `AttributeIntercept2` float NOT NULL AFTER `AttributeID2`,
  ADD COLUMN `AttributeSlope2` float NOT NULL AFTER `AttributeIntercept2`;

ALTER TABLE `cpu_battle_story_message`
  ADD COLUMN `OwnCharacterActingType` tinyint unsigned NOT NULL AFTER `OwnCharacterID`,
  ADD COLUMN `EnemyCharacterActingType` tinyint unsigned NOT NULL AFTER `EnemyCharacterID`,
  ADD COLUMN `EffectType` tinyint unsigned NOT NULL AFTER `EnemyCharacterActingType`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `action`
  CHANGE COLUMN `AttributeID1` `AttributeID` int unsigned NOT NULL,
  CHANGE COLUMN `AttributeIntercept1` `AttributeIntercept` float NOT NULL,
  CHANGE COLUMN `AttributeSlope1` `AttributeSlope` float NOT NULL,
  DROP COLUMN `AttributeID2`,
  DROP COLUMN `AttributeIntercept2`,
  DROP COLUMN `AttributeSlope2`;

ALTER TABLE `cpu_battle_story_message`
  DROP COLUMN `OwnCharacterActingType`,
  DROP COLUMN `EnemyCharacterActingType`,
  DROP COLUMN `EffectType`;

