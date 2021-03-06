<?php
	// Initialize session if not already
	if (!isset($_SESSION)) {
		session_start();
	}

	// Check if client is in a session
	if (!isset($_SESSION['user'])) {
		header('Location: index.php');
	}
?>
<!DOCTYPE html>
<html lang="es">

<head>
  <link rel="icon" href="./assets/images/m2go.png">
  <title>Metrópoli2Go - Kanban</title>

  <!-- Meta -->
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />

  <!-- CSS -->
	<link rel="stylesheet" href="./css/libs/jquery-WaitMe.min.css">
	<link rel="stylesheet" href="./css/libs/bootstrap.min.css">
	<link rel="stylesheet" href="./css/libs/bootstrap-Switch.min.css">
  <link rel="stylesheet" href="./css/libs/dragula.min.css">

  <link rel="stylesheet" href="./css/kanban.css">

	<style>
		.unselectable {
			user-select: none;
      -moz-user-select: none;
      -khtml-user-select: none;
      -webkit-user-select: none;
      -o-user-select: none;
		}

		.modal-lg {
    	max-width: 80% !important;
    	max-height: 80% !important;
		}

		.center {
			text-align: center;
		}
	</style>
</head>

<body>

  <?php include "./php/export.php" ?>
  <?php include "./php/settings.php" ?>
	<?php include "./php/sections.php" ?>
	<?php include "./php/landmarks.php" ?>

	<!-- NAVIGATION MENU -->
	<nav class="navbar navbar-expand-lg navbar-light sticky-top bg-light">
		<a href="#" class="navbar-brand"><img src="./assets/images/m2go_full.png" style="max-width: 135px;"></a>
	  <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbar" aria-controls="navbar" aria-expanded="false">
	    <span class="navbar-toggler-icon"></span>
	  </button>

	  <div class="collapse navbar-collapse" id="navbar">
	    <ul class="navbar-nav mr-auto">
				<!-- SECTIONS -->
	      <li class="nav-item">
	        <a onclick="$('#section_add').modal('show');" class="nav-link" href="#">+ Agregar Sección</a>
	      </li>

				<!-- MAP -->
	      <li class="nav-item">
	        <a onclick="window.open('./assets/images/map.jpg', 'Metrópoli2Go - Mapa', 'width=800, height=600');" class="nav-link" href="#">🗺️ Ver el Mapa</a>
	      </li>
	    </ul>

	    <div class="form-inline my-2 my-lg-0">
	      <input class="form-control mr-sm-2" type="text" oninput="VUE_ELEMENTS.kanban.search_term = this.value;" placeholder="🔍 Buscar..." aria-label="🔍 Buscar...">
				<div class="btn-group mr-sm-2" role="group">
					<button onclick="refresh('kanban');" class="form-control btn btn-outline-secondary my-2 my-sm-0">↻ Refrescar</button>
					<button onclick="$('#export_view').modal('show');" class="form-control btn btn-outline-secondary my-2 my-sm-0">⇱ Exportar</button>
				</div>
				<div class="btn-group mr-sm-2" role="group">
		      <button onclick="$('#settings').modal('show');" class="form-control btn btn-outline-success my-2 my-sm-0" type="submit">Ajustes</button>
		      <button onclick="logout();" class="form-control btn btn-outline-success my-2 my-sm-0" type="submit">Cerrar Sesión</button>
				</div>
	    </div>
	  </div>
	</nav>

	<!-- KANBAN -->
	<div class="kanban-parent" id="kanban">
		<kanban-component :search_term="search_term" :data="data"></kanban-component>
	</div>

	<!-- INFO BANNER -->
	<div class="alert alert-secondary" id="info-banner">
  	<label class="font-weight-bold">Número de Secciónes:</label>
		<span class="font-italic" id="info-banner-sectiones">0</span>
  	<label class="font-weight-bold">Número de Landmarks:</label>
		<span class="font-italic" id="info-banner-landmarks">0</span>
	</div>

  <!-- Javascript -->
  <script src="./js/libs/jquery-3.3.1.min.js"></script>
  <script src="./js/libs/jquery-FileDrop.js"></script>
  <script src="./js/libs/jquery-WaitMe.min.js"></script>
  <script src="./js/libs/bootstrap.min.js"></script>
  <script src="./js/libs/vue.min.js"></script>
  <script src="./js/libs/dragula.min.js"></script>
  <script src="./js/libs/html2pdf.bundle.min.js"></script>

  <script src="./js/common.js"></script>
  <script src="./js/kanban.js"></script>

	<!-- VUE Templates -->
	<script type="text/x-template" id="table-component">
	  <table class="table table-striped table-hover table-sm">
	    <thead>
		    <tr>
		      <th v-for="(val, key) in columns" @click="val.order = (val.order === 'des') ? 'asc' : 'des'; sort_key = key;" style="cursor: pointer;" class="unselectable" :class="{ active: sort_key == key }">
						{{ key | capitalize }}
						<span v-if="key === sort_key && val.order === 'des'">↑</span>
						<span v-if="key === sort_key && val.order === 'asc'">↓</span>
		      </th>
		      <th v-if="modifiable || removable" class="unselectable">
						Acciónes
		      </th>
		    </tr>
	    </thead>
	    <tbody>
	      <tr v-for="entry in filtered_data" @click="select($event, entry);">
	        <td v-for="(val, key) in columns">
						<span> {{ entry[val.referencing] }} </span>
	        </td>
					<td v-if="modifiable || removable">
						<span v-if="modifiable" @click="edit($event);" style="cursor: pointer;">✏️</span>
						<span v-if="removable" @click="remove($event);" style="cursor: pointer;">🗑️</span>
					</td>
	      </tr>
  		</tbody>
		</table>
	</script>


	<script type="text/x-template" id="export-component">
	  <table class="table table-striped table-hover table-sm table-responsive-sm">
	    <thead>
		    <tr>
		      <th v-for="(val, column) in columns" class="unselectable" :class="{ 'text-center': column !== 'Nombre'}">
						{{ column }}
		      </th>
		    </tr>
	    </thead>
	    <tbody>
	      <tr v-for="landmark in filtered_data">
	        <td v-for="(val, column) in columns" :class="{'text-center': column !== 'Nombre'}">
						<span v-if="column === 'Nombre'"> {{ landmark.name }} </span>
						<span v-if="landmark.classification === column">◉</span>
	        </td>
	      </tr>
				<tr>
					<td v-for="(val, column) in columns" class="text-center">
						<b v-if="column in count"> {{ count[column] }} </b>
						<b v-else>0</b>
					</td>
				</tr>
  		</tbody>
		</table>
	</script>


	<script type="text/x-template" id="kanban-component">
		<div class="kanban">
			<div v-for="section in filtered_data" :data-section-id="section.id" @click="GLOBALS.section = Object.assign({}, section);" class="section shadow-sm">
				<div class="header border-bottom">
					<a href="#" onclick="$('#section_modify').modal('show');" class="name btn-link border-0"> {{ section.name | capitalize }} </a> <br />
					<input type="button" onclick="$('#landmark_add').modal('show');" value="+ Agregar Landmark" class="btn btn-primar" />
				</div>
				<div class="landmarks border">
					<div v-for="landmark in section.data" onclick="$('#landmark_modify').modal('show');" :data-landmark-id="landmark.id" @click="GLOBALS.landmark = Object.assign({}, landmark); VUE_ELEMENTS.files.files = landmark.files;" class="landmark shadow">
						<a href="#" class="name btn-link border-0"> {{ landmark.name | capitalize }} </a>
					</div>
				</div>
			</div>
		</div>
	</script>


	<script type="text/x-template" id="files-component">
		<div class="files">
			<div v-for="file in files" style="margin: 5px;" class="file btn-group border rounded" role="group">
				<a href="#" @click="window.open(file.url);" class="btn-sm border-0"> {{ file['visual name'] | capitalize }} </a>
				<button @click="files_remove(file.id);" class="btn btn-danger">X</button>
			</div>
		</div>
	</script>

</body>

</html>
