SET @@DEFAULT_STORAGE_ENGINE = 'InnoDB';
SET @@AUTO_INCREMENT_OFFSET = 1;
SET @@AUTO_INCREMENT_INCREMENT = 1;
SET @@UNIQUE_CHECKS=0;
SET @@FOREIGN_KEY_CHECKS=0;
SET @@SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

DROP SCHEMA IF EXISTS `log`;
CREATE SCHEMA `log` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `log`;

CREATE TABLE `log`.`error_log` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `content` VARCHAR(500) NOT NULL,
  PRIMARY KEY (`id`));