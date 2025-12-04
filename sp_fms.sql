-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Dec 04, 2025 at 09:21 AM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `sp_fms`
--

-- --------------------------------------------------------

--
-- Table structure for table `expenses`
--

CREATE TABLE `expenses` (
  `expense_id` int(11) NOT NULL,
  `student_id` varchar(20) NOT NULL,
  `category` varchar(100) NOT NULL,
  `cost` decimal(10,2) NOT NULL,
  `date_added` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `expenses`
--

INSERT INTO `expenses` (`expense_id`, `student_id`, `category`, `cost`, `date_added`) VALUES
(16, '24-1879', 'Food', 60.00, '2025-11-29'),
(17, '24-1879', 'Transportation', 50.00, '2025-11-29'),
(18, '24-1879', 'Others', 15.00, '2025-11-29'),
(19, '24-1879', 'Food', 70.00, '2025-11-30'),
(20, '24-1879', 'Others', 15.00, '2025-11-30'),
(26, '24-1879', 'Food', 50.00, '2025-12-01'),
(27, '24-1879', 'Transportation', 20.00, '2025-12-01'),
(28, '24-1879', 'Food', 20.00, '2025-12-01'),
(29, '24-1879', 'Transportation', 30.00, '2025-12-03'),
(30, '24-1879', 'Food', 70.00, '2025-12-03'),
(31, '24-1879', 'Others', 20.00, '2025-12-03'),
(40, '24-2222', 'Food', 50.00, '2025-12-04'),
(41, '24-2222', 'Transportation', 100.00, '2025-12-04'),
(42, '24-2222', 'Others', 12.00, '2025-12-04'),
(43, '24-1234', 'Food', 100.00, '2025-12-04'),
(44, '24-1234', 'Transportation', 150.00, '2025-12-04');

-- --------------------------------------------------------

--
-- Table structure for table `students`
--

CREATE TABLE `students` (
  `id` varchar(20) NOT NULL,
  `first_name` varchar(50) NOT NULL,
  `last_name` varchar(50) NOT NULL,
  `course` varchar(50) NOT NULL,
  `email` varchar(100) NOT NULL,
  `contact` varchar(20) NOT NULL,
  `password` varchar(255) NOT NULL,
  `notes` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `students`
--

INSERT INTO `students` (`id`, `first_name`, `last_name`, `course`, `email`, `contact`, `password`, `notes`) VALUES
('24-1000', 'test3', 'test3', 'sfit-2b', 'test3@gmail.com', '0967163713', 'dYIMeKiqa9syH0JIykAJYA==:r/YZgPoAWxH7ffj7tpRVDwgtjjhDVDxmk+xbgLBzBvM=', NULL),
('24-1111', 'test1', 'test1', 'SFIT-2B', 'test1@gmail.com', '0977771231', 'q3Afr8R2GElP7RFxXUmnrw==:QDwKvf/qEDHJJ0gQy9BvkkfebuK+f2tU9SSL05IJ5OI=', NULL),
('24-1234', 'test1', 'test2', 'sfit-2b', 'test22@gmail.com', '091231311231', 'F2vmH2G7JjSrBCZR1v4UxA==:RjRqXinWoSRWHPeG7crFMmu7ZXpy17dG/CUbUuJDeh0=', NULL),
('24-1877', 'Rich', 'Dego', 'SFIT-2B', 'richdego@gmail.com', '09123781312', 'P3htwbN1NNKqdQdmE4WXWw==:o6iWoZsBdcF703Eovo5u5i8SoktxmtQU+71ZfrmYBlg=', NULL),
('24-1879', 'Ries', 'Riyas', 'SFIT - 2B', 'Riesyas@gmail.com', '0912313123', 'rfJso2HK1kNxJTr8IXJMXw==:IA7h7TAG2sAE+wMmCrKAQvSITVjT4srpPX4NAAaLm/Q=', NULL),
('24-2222', 'test4', 'test4', 'sfit-2b', 'test4@gmail.com', '0917361723', 'giExtoEtkc+ZLkgBVfXXsA==:/hEgZf9Ra4sxmwl7dUVQCwiKZlLGNQjrnn0lKHfgGmI=', NULL),
('24-8888', 'Real', 'Fake', 'SFIT-2B', 'Realfake@gmail.com', '09999123123', 'mKXkzrCde5Xw5FswiOrHCw==:UbKOSLjTUWBxyZKV+/S+4FX02aNKaD3cTylsYVhsC9w=', NULL),
('24-9999', 'test', 'test', 'SFIT-2B', 'testtest@gmail.com', '09888811231', 'Axi+b9oSMOkb1WVhsBMJfg==:9SPAgOekaV2NNwRfyerebQ8A789PXouJO/JR0ECzpjQ=', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `student_budget`
--

CREATE TABLE `student_budget` (
  `budget_id` int(11) NOT NULL,
  `student_id` varchar(20) NOT NULL,
  `budget_amount` decimal(10,2) NOT NULL,
  `date_set` date NOT NULL,
  `budget_remaining` decimal(10,2) NOT NULL DEFAULT 0.00
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `student_budget`
--

INSERT INTO `student_budget` (`budget_id`, `student_id`, `budget_amount`, `date_set`, `budget_remaining`) VALUES
(1, '24-1879', 500.00, '2025-11-23', 200.00),
(2, '24-1879', 500.00, '2025-11-24', 150.00),
(3, '24-1879', 150.00, '2025-11-25', 50.00),
(4, '24-1879', 130.00, '2025-11-26', 65.00),
(5, '24-1879', 200.00, '2025-11-29', 75.00),
(6, '24-1879', 100.00, '2025-11-30', 15.00),
(7, '24-1879', 200.00, '2025-12-01', 110.00),
(8, '24-1879', 200.00, '2025-12-03', 80.00),
(9, '24-1111', 200.00, '2025-12-04', 200.00),
(10, '24-1000', 150.00, '2025-12-04', 150.00),
(11, '24-2222', 200.00, '2025-12-04', 38.00),
(12, '24-1234', 500.00, '2025-12-04', 250.00);

-- --------------------------------------------------------

--
-- Table structure for table `todo_progress`
--

CREATE TABLE `todo_progress` (
  `progress_id` int(11) NOT NULL,
  `student_id` varchar(20) DEFAULT NULL,
  `date_recorded` date DEFAULT NULL,
  `completed_tasks` int(11) DEFAULT NULL,
  `total_tasks` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `todo_progress`
--

INSERT INTO `todo_progress` (`progress_id`, `student_id`, `date_recorded`, `completed_tasks`, `total_tasks`) VALUES
(18, '24-1877', '2025-11-23', 1, 3),
(22, '24-1879', '2025-11-29', 5, 9),
(23, '24-1879', '2025-11-30', 6, 11),
(24, '24-1879', '2025-12-01', 6, 13),
(25, '24-1879', '2025-12-02', 5, 12),
(26, '24-1879', '2025-12-03', 2, 4),
(27, '24-1111', '2025-12-04', 0, 0),
(28, '24-1879', '2025-12-04', 0, 0),
(29, '24-1000', '2025-12-04', 0, 0),
(30, '24-2222', '2025-12-04', 1, 3),
(31, '24-1234', '2025-12-04', 1, 2);

-- --------------------------------------------------------

--
-- Table structure for table `todo_tasks`
--

CREATE TABLE `todo_tasks` (
  `task_id` int(11) NOT NULL,
  `student_id` varchar(20) DEFAULT NULL,
  `task_name` varchar(255) DEFAULT NULL,
  `is_completed` tinyint(1) DEFAULT 0,
  `completion_date` date DEFAULT NULL,
  `created_date` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `todo_tasks`
--

INSERT INTO `todo_tasks` (`task_id`, `student_id`, `task_name`, `is_completed`, `completion_date`, `created_date`) VALUES
(30, '24-1877', 'Activity 1', 1, '2025-11-23', NULL),
(33, '24-1877', 'Activity 2', 0, NULL, NULL),
(34, '24-1877', 'Activity 3', 0, NULL, NULL),
(73, '24-2222', '1', 1, '2025-12-04', '2025-12-04'),
(75, '24-2222', '2', 0, NULL, '2025-12-04'),
(76, '24-2222', '3', 0, NULL, '2025-12-04'),
(77, '24-1234', '1', 1, '2025-12-04', '2025-12-04'),
(78, '24-1234', '2', 0, NULL, '2025-12-04');

-- --------------------------------------------------------

--
-- Table structure for table `weekly_records`
--

CREATE TABLE `weekly_records` (
  `record_id` int(11) NOT NULL,
  `student_id` varchar(20) NOT NULL,
  `week_start_date` date NOT NULL,
  `week_end_date` date NOT NULL,
  `completed_tasks` int(11) NOT NULL,
  `total_tasks` int(11) NOT NULL,
  `food_total` decimal(10,2) NOT NULL DEFAULT 0.00,
  `food_last_date` date DEFAULT NULL,
  `transportation_total` decimal(10,2) NOT NULL DEFAULT 0.00,
  `transportation_last_date` date DEFAULT NULL,
  `others_total` decimal(10,2) NOT NULL DEFAULT 0.00,
  `others_last_date` date DEFAULT NULL,
  `total_budget` decimal(10,2) NOT NULL DEFAULT 0.00,
  `total_balance` decimal(10,2) NOT NULL DEFAULT 0.00,
  `budget_last_date` date DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `weekly_records`
--

INSERT INTO `weekly_records` (`record_id`, `student_id`, `week_start_date`, `week_end_date`, `completed_tasks`, `total_tasks`, `food_total`, `food_last_date`, `transportation_total`, `transportation_last_date`, `others_total`, `others_last_date`, `total_budget`, `total_balance`, `budget_last_date`, `created_at`) VALUES
(1, '24-1879', '2025-11-26', '2025-12-02', 5, 12, 230.00, '2025-12-01', 90.00, '2025-12-01', 45.00, '2025-11-30', 200.00, 0.00, '2025-12-01', '2025-12-02 05:09:52'),
(2, '24-1879', '2025-11-25', '2025-12-01', 6, 13, 290.00, '2025-12-01', 110.00, '2025-12-01', 65.00, '2025-11-30', 200.00, 0.00, '2025-12-01', '2025-12-02 05:38:08'),
(3, '24-1111', '2025-11-25', '2025-12-01', 0, 0, 0.00, NULL, 0.00, NULL, 0.00, NULL, 0.00, 0.00, NULL, '2025-12-04 07:42:35'),
(4, '24-1000', '2025-11-25', '2025-12-01', 0, 0, 0.00, NULL, 0.00, NULL, 0.00, NULL, 0.00, 0.00, NULL, '2025-12-04 07:52:35'),
(5, '24-2222', '2025-12-04', '2025-12-10', 0, 1, 0.00, NULL, 0.00, NULL, 0.00, NULL, 0.00, 0.00, NULL, '2025-12-04 08:06:51'),
(6, '24-1234', '2025-12-04', '2025-12-10', 0, 1, 0.00, NULL, 0.00, NULL, 0.00, NULL, 0.00, 0.00, NULL, '2025-12-04 08:18:44');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `expenses`
--
ALTER TABLE `expenses`
  ADD PRIMARY KEY (`expense_id`),
  ADD KEY `student_id` (`student_id`);

--
-- Indexes for table `students`
--
ALTER TABLE `students`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `student_budget`
--
ALTER TABLE `student_budget`
  ADD PRIMARY KEY (`budget_id`),
  ADD UNIQUE KEY `unique_student_date` (`student_id`,`date_set`);

--
-- Indexes for table `todo_progress`
--
ALTER TABLE `todo_progress`
  ADD PRIMARY KEY (`progress_id`),
  ADD KEY `student_id` (`student_id`);

--
-- Indexes for table `todo_tasks`
--
ALTER TABLE `todo_tasks`
  ADD PRIMARY KEY (`task_id`),
  ADD KEY `student_id` (`student_id`);

--
-- Indexes for table `weekly_records`
--
ALTER TABLE `weekly_records`
  ADD PRIMARY KEY (`record_id`),
  ADD UNIQUE KEY `unique_student_week` (`student_id`,`week_end_date`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `expenses`
--
ALTER TABLE `expenses`
  MODIFY `expense_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=45;

--
-- AUTO_INCREMENT for table `student_budget`
--
ALTER TABLE `student_budget`
  MODIFY `budget_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT for table `todo_progress`
--
ALTER TABLE `todo_progress`
  MODIFY `progress_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=32;

--
-- AUTO_INCREMENT for table `todo_tasks`
--
ALTER TABLE `todo_tasks`
  MODIFY `task_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=79;

--
-- AUTO_INCREMENT for table `weekly_records`
--
ALTER TABLE `weekly_records`
  MODIFY `record_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `expenses`
--
ALTER TABLE `expenses`
  ADD CONSTRAINT `expenses_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`);

--
-- Constraints for table `student_budget`
--
ALTER TABLE `student_budget`
  ADD CONSTRAINT `student_budget_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`);

--
-- Constraints for table `todo_progress`
--
ALTER TABLE `todo_progress`
  ADD CONSTRAINT `todo_progress_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`);

--
-- Constraints for table `todo_tasks`
--
ALTER TABLE `todo_tasks`
  ADD CONSTRAINT `todo_tasks_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`);

--
-- Constraints for table `weekly_records`
--
ALTER TABLE `weekly_records`
  ADD CONSTRAINT `weekly_records_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
