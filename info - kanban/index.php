<!DOCTYPE html>
<html>

<head>

  <!-- Setup the icon for the page -->
  <link rel="icon" href="./assets/icon.png">

  <title>Metropoli2Go</title>

  <!-- Meta -->
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />

  <!-- CSS -->
  <link rel="stylesheet" href="./css/common/gui.css">
  <link rel="stylesheet" href="./css/index.css">
  <link rel="stylesheet" href="./css/lib/jquery-ui.min.css">
  <link rel="stylesheet" href="./css/lib/jquery-ui-theme.css">
  <link rel="stylesheet" href="./css/lib/dragula.min.css">

  <!-- Javascript -->
  <script src="./js/lib/jquery-3.3.1.min.js"></script>
  <script src="./js/lib/jquery-ui.min.js"></script>
  <script src="./js/lib/jquery-ui-touch-punch.min.js"></script>
  <script src="./js/lib/dragula.min.js"></script>
  <script src="./js/common/gui.js"></script>

</head>

<body>

  <!-- TODO Login before continuing to load -->
  <?php



  ?>
  <?php include "./php/configuration_menu.php" ?>
  <?php include "./php/category_menu.php" ?>
  <?php include "./php/task_menu.php" ?>

  <!-- Navigation Bar -->
  <div class="nav">
    <!-- Menu -->
    <div class="menu">
      <span class="title">
        Metropoli2Go
      </span>
      <span class="user selectable" onclick="$('#Configuration_Menu').dialog('open');" id="user">
        ClarkThyLord
      </span>
    </div>
    <!-- Toolbar -->
    <div class="toolbar">
      <label>
        Search:
        <input type="text" class="tool selectable" placeholder="Search term..." />
      </label>
      <label>
        Filter:
        <input type="text" class="tool selectable" placeholder="Filter by..." />
      </label>
      <span class="tool selectable" onclick="$('#Task_Menu').dialog('open');">
        &#9881;
      </span>
      <span class="tool selectable">
        Add +
      </span>
    </div>
  </div>

  <!-- Workspace -->
  <div class="workspace">
    <!-- prev and next controls -->
    <a class="prev selectable">
      &#10094;
    </a>
    <a class="next selectable">
      &#10095;
    </a>
    <!-- Kanban -->
    <div class="kanban">
    </div>
  </div>

</body>

</html>
