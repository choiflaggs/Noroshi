
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `action`
  CHANGE COLUMN `DamageType` `DamageType` tinyint unsigned NOT NULL,
  CHANGE COLUMN `DamagePhysicalAttribute` `DamagePhysicalAttribute` tinyint unsigned NOT NULL,
  CHANGE COLUMN `DamageMagicalAttribute` `DamageMagicalAttribute` tinyint unsigned NOT NULL,
  ADD COLUMN `ExecutableProbability` float unsigned NOT NULL AFTER `TriggerID`,
  ADD COLUMN `ExecutableNum` tinyint unsigned NOT NULL AFTER `ExecutableProbability`;

ALTER TABLE `attribute`
  CHANGE COLUMN `Arg1` `Arg1` int NOT NULL,
  CHANGE COLUMN `Arg2` `Arg2` int NOT NULL,
  CHANGE COLUMN `Arg3` `Arg3` int NOT NULL,
  CHANGE COLUMN `Arg4` `Arg4` int NOT NULL,
  CHANGE COLUMN `Arg5` `Arg5` int NOT NULL,
  CHANGE COLUMN `Arg6` `Arg6` int NOT NULL,
  CHANGE COLUMN `Arg7` `Arg7` int NOT NULL,
  CHANGE COLUMN `Arg8` `Arg8` int NOT NULL,
  CHANGE COLUMN `Arg9` `Arg9` int NOT NULL,
  CHANGE COLUMN `Arg10` `Arg10` int NOT NULL;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `action`
  CHANGE COLUMN `DamageType` `DamageType` tinyint NOT NULL,
  CHANGE COLUMN `DamagePhysicalAttribute` `DamagePhysicalAttribute` tinyint NOT NULL,
  CHANGE COLUMN `DamageMagicalAttribute` `DamageMagicalAttribute` tinyint NOT NULL,
  DROP COLUMN `ExecutableProbability`,
  DROP COLUMN `ExecutableNum`;

ALTER TABLE `attribute`
  CHANGE COLUMN `Arg1` `Arg1` int unsigned NOT NULL,
  CHANGE COLUMN `Arg2` `Arg2` int unsigned NOT NULL,
  CHANGE COLUMN `Arg3` `Arg3` int unsigned NOT NULL,
  CHANGE COLUMN `Arg4` `Arg4` int unsigned NOT NULL,
  CHANGE COLUMN `Arg5` `Arg5` int unsigned NOT NULL,
  CHANGE COLUMN `Arg6` `Arg6` int unsigned NOT NULL,
  CHANGE COLUMN `Arg7` `Arg7` int unsigned NOT NULL,
  CHANGE COLUMN `Arg8` `Arg8` int unsigned NOT NULL,
  CHANGE COLUMN `Arg9` `Arg9` int unsigned NOT NULL,
  CHANGE COLUMN `Arg10` `Arg10` int unsigned NOT NULL;

