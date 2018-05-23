<?php

	/**
	* Get section(s) from SQL datbase that fit the given filter.
	* @param filter [array] Properties to get section(s) with.
	* @param options [array] Options to get section(s) with.
	* @return {undefined} Returns nothing.
	*/
	function section_get($filter=array(), $options=array()) {
		$filter_sql = '';
		foreach ($filter as $key => $value) {
			$filter_sql .= " AND `{$key}` = '{$value}'";
		}

		$options_sql = '';
		foreach ($options as $key => $value) {
			switch ($key) {
				case 'limit':
				$options_sql .= " LIMIT {$value}";
					break;
			}
		}

		$sql = "SELECT * FROM `sections` WHERE 1 {$filter_sql} ORDER BY `created` DESC {$options_sql}";

		// FOR DEBUGGING
		if (is_debugging()) {
			array_push($GLOBALS['response']['debug']['database']['sql'], $sql);
		}

    $sections = $GLOBALS['conn']->query($sql);
    if ($sections->num_rows > 0) {
      $GLOBALS['response']['data']['dump'] = array();
      while($section = $sections->fetch_assoc()) {
        array_push($GLOBALS['response']['data']['dump'], $section);
      }

			response_status(true, 'se encontró sección(s) válida(s)');
    } else {
			response_status(true, 'no se encontró sección(s) válida(s)');
    }
	}


	/**
	* Add a section to the SQL database with the given data.
	* @param data [array] Data to create section with.
	* @return {undefined} Returns nothing.
	*/
	function section_add($data=array()) {
    access_check(1);

		$data = array_merge(array('name' => 'Nueva Sección'), $data);

		foreach ($data as $key => $value) {
			if (gettype($value) === 'string') {
				$data[$key] = str_replace("'", "''", $value);
			}
		}

    $sql = "INSERT INTO `sections` (`id`, `name`) VALUES (NULL, '{$name}')";

		// FOR DEBUGGING
		if (is_debugging()) {
			array_push($GLOBALS['response']['debug']['database']['sql'], $sql);
		}

    if ($GLOBALS['conn']->query($sql) === TRUE) {
      $insert_id = $GLOBALS['conn']->insert_id;

      $sql = "SELECT * FROM `sections` WHERE `id` = '{$insert_id}' LIMIT 1";

			// FOR DEBUGGING
			if (is_debugging()) {
				array_push($GLOBALS['response']['debug']['database']['sql'], $sql);
			}

      $GLOBALS['response']['data']['section'] = $GLOBALS['conn']->query($sql)->fetch_assoc();

			// LOG
			qlog($_SESSION['user']['id'], 'sección creado', 'sections', "{$insert_id}");

			section_get();

			response_send(true, 'sección agregado exitosamente');
    } else {
			response_send(false, 'sección agregado sin éxito');
    }
	}


	/**
	* Modify a section in the SQL database.
	* @param section_id [string] ID of section to be modified.
	* @param data [array] Data to modify section with.
	* @return {undefined} Returns nothing.
	*/
	function section_modify($section_id=0, $data=array()) {
		access_check(1);
		if ($section_id <= 0) {
			response_send(false, 'la identificación del sección no fue dada');
		}
		if (sizeof($data) === 0) {
			response_send(false, 'datos, para modificar el sección con, no se dio');
		}

		$data_sql = array();
		$valid_keys = array('name');
		foreach ($data as $key => $value) {
			if (!in_array($key, $valid_keys)) { continue; } else {
				if (gettype($value) === 'array') {
					array_push($data_sql, "`{$key}` = '" . str_replace("'", "''", json_encode($value)) . "'");
				} else {
					array_push($data_sql, "`{$key}` = '" . str_replace("'", "''", $value) . "'");
				}
			}
		}

		$sql = "UPDATE `sections` SET " . join(', ', $data_sql) . " WHERE `id` = {$section_id}";

		// FOR DEBUGGING
		if (is_debugging()) {
			array_push($GLOBALS['response']['debug']['database']['sql'], $sql);
		}

		if ($GLOBALS["conn"]->query($sql) === true) {
			// LOG
			qlog($_SESSION['user']['id'], 'sección modificado', 'sections', "{$section_id}");

			section_get();

			response_send(true, 'sección modificado con éxito');
		} else {
			response_send(false, 'sección modificado sin éxito');
		}
	}


	/**
	* Remove a section from the SQL database.
	* @param section_id [string] ID of section to be removed.
	* @return {undefined} Returns nothing.
	*/
	function section_remove($section_id=0) {
		access_check(1);
		if ($section_id === 0) {
		 response_send(false, 'la identificación del usuario no fue dada');
		}

    $sql = "DELETE FROM `sections` WHERE `id` = {$section_id}";

		// FOR DEBUGGING
		if (is_debugging()) {
		 array_push($GLOBALS['response']['debug']['database']['sql'], $sql);
		}

		if ($GLOBALS['conn']->query($sql) == True) {
      $GLOBALS['response']['data']['id'] = $section_id;

			// LOG
			qlog($_SESSION['user']['id'], 'sección eliminado', 'sections', "{$section_id}");

			section_get();

			response_send(true, 'sección eliminado con éxito');
    } else {
			response_send(false, 'sección eliminado sin éxito');
    }
	}

?>
