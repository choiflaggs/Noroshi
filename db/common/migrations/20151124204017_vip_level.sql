
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
CREATE TABLE `player_vip_level_bonus` (
  `Level` SMALLINT UNSIGNED NOT NULL,
  `QuickBattle` TINYINT UNSIGNED NOT NULL,
  `ConsecutiveQuickBattles` TINYINT UNSIGNED NOT NULL,
  `GemToGearEnchant` TINYINT UNSIGNED NOT NULL,
  `VipGacha` TINYINT UNSIGNED NOT NULL,
  `MaxStaminaRecoveryNum` TINYINT UNSIGNED NOT NULL,
  `MaxBPRecoveryNum` SMALLINT UNSIGNED NOT NULL,
  `MaxBackStoryRecoveryNum` TINYINT UNSIGNED NOT NULL,
  `MaxActionLevelPointRecoveryNum` TINYINT UNSIGNED NOT NULL,
  `MaxRentalCharacterNum` TINYINT UNSIGNED NOT NULL,
  `MaxResetExpeditionNum` TINYINT UNSIGNED NOT NULL,
  `MaxGreetingNum` TINYINT UNSIGNED NOT NULL,
  `GemBonus` FLOAT UNSIGNED NOT NULL,
  `GuildBossPointBonus` FLOAT UNSIGNED NOT NULL,
  `ExpeditionPointBonus` FLOAT UNSIGNED NOT NULL,
  `ArenaPointBonus` FLOAT UNSIGNED NOT NULL,
  `ExpeditionGoldBonus` FLOAT UNSIGNED NOT NULL,
  `DailyReceivableRaidTicketNum` SMALLINT UNSIGNED NOT NULL,
  PRIMARY KEY (`Level`));


-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
DROP TABLE `player_vip_level_bonus`;
