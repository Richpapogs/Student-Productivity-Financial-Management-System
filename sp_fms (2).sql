-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Nov 25, 2025 at 02:34 PM
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
(1, '24-1879', 'Food', 150.00, '2025-11-23'),
(2, '24-1879', 'Transportation', 100.00, '2025-11-23'),
(3, '24-1879', 'School Activity', 50.00, '2025-11-23'),
(5, '24-1879', 'Food', 130.00, '2025-11-24'),
(6, '24-1879', 'Transportation', 120.00, '2025-11-24'),
(8, '24-1879', 'Others', 100.00, '2025-11-24'),
(9, '24-1879', 'Food', 60.00, '2025-11-25'),
(10, '24-1879', 'Transportation', 20.00, '2025-11-25'),
(11, '24-1879', 'Others', 20.00, '2025-11-25');

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
  `password` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `students`
--

INSERT INTO `students` (`id`, `first_name`, `last_name`, `course`, `email`, `contact`, `password`) VALUES
('24-1877', 'Rich', 'Dego', 'SFIT-2B', 'richdego@gmail.com', '09123781312', 'P3htwbN1NNKqdQdmE4WXWw==:o6iWoZsBdcF703Eovo5u5i8SoktxmtQU+71ZfrmYBlg='),
('24-1879', 'Ries', 'Riyas', 'SFIT - 2B', 'Riesyas@gmail.com', '0912313123', 'iQ11QWZVchm60Ed4+4Xldw==:QlyHinxmYGMkFDjx0hFkH7wq/0Co2ghmNd7wJmnOHWM=');

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
(3, '24-1879', 150.00, '2025-11-25', 50.00);

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
(17, '24-1879', '2025-11-23', 1, 3),
(18, '24-1877', '2025-11-23', 1, 3),
(19, '24-1879', '2025-11-24', 2, 4),
(20, '24-1879', '2025-11-25', 3, 6);

-- --------------------------------------------------------

--
-- Table structure for table `todo_tasks`
--

CREATE TABLE `todo_tasks` (
  `task_id` int(11) NOT NULL,
  `student_id` varchar(20) DEFAULT NULL,
  `task_name` varchar(255) DEFAULT NULL,
  `is_completed` tinyint(1) DEFAULT 0,
  `completion_date` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `todo_tasks`
--

INSERT INTO `todo_tasks` (`task_id`, `student_id`, `task_name`, `is_completed`, `completion_date`) VALUES
(24, '24-1879', 'Activity 1', 1, '2025-11-23'),
(27, '24-1879', 'Activity 3', 0, NULL),
(29, '24-1879', 'Activity 4', 0, NULL),
(30, '24-1877', 'Activity 1', 1, '2025-11-23'),
(33, '24-1877', 'Activity 2', 0, NULL),
(34, '24-1877', 'Activity 3', 0, NULL),
(40, '24-1879', 'Activity 5', 1, '2025-11-24'),
(43, '24-1879', 'Activity 6', 1, '2025-11-25'),
(44, '24-1879', 'Activity 7', 0, NULL);

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
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `expenses`
--
ALTER TABLE `expenses`
  MODIFY `expense_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT for table `student_budget`
--
ALTER TABLE `student_budget`
  MODIFY `budget_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `todo_progress`
--
ALTER TABLE `todo_progress`
  MODIFY `progress_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT for table `todo_tasks`
--
ALTER TABLE `todo_tasks`
  MODIFY `task_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=47;

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
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
