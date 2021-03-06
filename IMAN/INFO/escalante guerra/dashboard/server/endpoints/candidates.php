<?php

	/**
	* Get candidate(s) from SQL datbase that fit the given filter.
	* @param filter [array] Properties to get candidate(s) with.
	* @param options [array] Options to get candidate(s) with.
	* @return {undefined} Returns nothing.
	*/
	function candidate_get($filter=array(), $options=array()) {
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

		$sql = "SELECT * FROM `candidates` WHERE 1 {$filter_sql} ORDER BY `created` DESC {$options_sql}";

		// FOR DEBUGGING
		if (is_debugging()) {
			array_push($GLOBALS['response']['debug']['database']['sql'], $sql);
		}

    $candidates = $GLOBALS['conn']->query($sql);
    if ($candidates->num_rows > 0) {
      $GLOBALS['response']['data']['dump'] = array();
      while($candidate = $candidates->fetch_assoc()) {
        array_push($GLOBALS['response']['data']['dump'], $candidate);
      }

			response_status(true, 'se encontró candidato(s) válido(s)');
    } else {
			response_status(true, 'no se encontró candidato(s) válido(s)');
    }
	}


	/**
	* Add a candidate to the SQL database with the given data.
	* @param data [array] Data to create candidate with.
	* @return {undefined} Returns nothing.
	*/
	function candidate_add($data=array()) {
		$data = array_merge(array('name' => 'Nueva Candidato', 'data' => 'NULL'), $data);
		if (gettype($data['data']) !== 'array') {
			$data['data'] = 'NULL';
		}else {
			$data['data'] = json_encode($data["data"]);
		}

		foreach ($data as $key => $value) {
			if (gettype($value) === 'string') {
				$data[$key] = str_replace("'", "''", $value);
			}
		}

		if ($data['data'] !== 'NULL') {
			$data['data'] = "'" . $data['data'] . "'";
		}

    $sql = "INSERT INTO `candidates` (`id`, `created`, `name`, `data`, `notes`, `private`, `active`) VALUES (NULL, CURRENT_TIMESTAMP, '{$data["name"]}', {$data["data"]}, '', '0', '1')";

		// FOR DEBUGGING
		if (is_debugging()) {
			array_push($GLOBALS['response']['debug']['database']['sql'], $sql);
		}

    if ($GLOBALS['conn']->query($sql) === TRUE) {
      $insert_id = $GLOBALS['conn']->insert_id;

      $sql = "SELECT * FROM `candidates` WHERE `id` = '{$insert_id}' LIMIT 1";

			// FOR DEBUGGING
			if (is_debugging()) {
				array_push($GLOBALS['response']['debug']['database']['sql'], $sql);
			}

      $GLOBALS['response']['data']['candidate'] = $GLOBALS['conn']->query($sql)->fetch_assoc();

			// LOG
			if (isset($_SESSION['user'])) {
				qlog($_SESSION['user']['id'], 'cotización creado', 'quotations', "{$insert_id}");
			} else {
				qlog($_SERVER['REMOTE_ADDR'], 'cotización creado', 'quotations', "{$insert_id}");
			}

			candidate_get();

			response_send(true, 'candidato agregado exitosamente');
    } else {
			response_send(false, 'candidato agregado sin éxito');
    }
	}


	/**
	* Modify a candidate in the SQL database.
	* @param candidate_id [string] ID of candidate to be modified.
	* @param data [array] Data to modify candidate with.
	* @return {undefined} Returns nothing.
	*/
	function candidate_modify($candidate_id=0, $data=array()) {
		access_check(1);
		if ($candidate_id <= 0) {
			response_send(false, 'la identificación del candidato no fue dada');
		}
		if (sizeof($data) === 0) {
			response_send(false, 'datos, para modificar el candidato con, no se dio');
		}

		$data_sql = array();
		$valid_keys = array('name', 'data', 'notes', 'private', 'active');
		foreach ($data as $key => $value) {
			if (!in_array($key, $valid_keys)) { continue; } else {
				if (gettype($value) === 'array') {
					array_push($data_sql, "`{$key}` = '" . str_replace("'", "''", json_encode($value)) . "'");
				} else {
					array_push($data_sql, "`{$key}` = '" . str_replace("'", "''", $value) . "'");
				}
			}
		}

		$sql = "UPDATE `candidates` SET " . join(', ', $data_sql) . " WHERE `id` = {$candidate_id}";

		// FOR DEBUGGING
		if (is_debugging()) {
			array_push($GLOBALS['response']['debug']['database']['sql'], $sql);
		}

		if ($GLOBALS["conn"]->query($sql) === true) {
			// LOG
			qlog($_SESSION['user']['id'], 'candidato modificado', 'candidates', "{$candidate_id}");

			candidate_get();

			response_send(true, 'candidato modificado con éxito');
		} else {
			response_send(false, 'candidato modificado sin éxito');
		}
	}


	/**
	* Modify private of a candidate in the SQL database.
	* @param candidate_id [string] ID of candidate to be modified.
	* @return {undefined} Returns nothing.
	*/
	function candidate_private($candidate_id=0) {
		access_check(1);
		if ($candidate_id <= 0) {
			response_send(false, 'la identificación del candidato no fue dada');
		}

		$GLOBALS['response']['files'] = $_FILES;

		// Create file on server
		if (count($_FILES) > 0) {
			foreach ($_FILES as $file) {
				$file_content = file_get_contents($file["tmp_name"]);
				$file_extension = pathinfo($file["name"], PATHINFO_EXTENSION);
	      file_put_contents("../privates/" . $candidate_id . "." . pathinfo($file["name"], PATHINFO_EXTENSION), $file_content);
				break;
			}
		}

		$sql = "UPDATE `candidates` SET `private` = '1'  WHERE `id` = {$candidate_id}";

		// FOR DEBUGGING
		if (is_debugging()) {
			array_push($GLOBALS['response']['debug']['database']['sql'], $sql);
		}

		if ($GLOBALS["conn"]->query($sql) === true) {
			// LOG
			qlog($_SESSION['user']['id'], 'privado de candidato modificado', 'candidates', "{$candidate_id}");

			candidate_get();

			response_send(true, 'privado de candidato modificado con éxito');
		} else {
			response_send(false, 'privado de candidato modificado sin éxito');
		}
	}


	/**
	* Remove a candidate from the SQL database.
	* @param candidate_id [string] ID of candidate to be removed.
	* @return {undefined} Returns nothing.
	*/
	function candidate_remove($candidate_id=0) {
		access_check(1);
		if ($candidate_id === 0) {
		 response_send(false, 'la identificación del usuario no fue dada');
		}

    $sql = "DELETE FROM `candidates` WHERE `id` = {$candidate_id}";

		// FOR DEBUGGING
		if (is_debugging()) {
		 array_push($GLOBALS['response']['debug']['database']['sql'], $sql);
		}

		if ($GLOBALS['conn']->query($sql) == True) {
      $GLOBALS['response']['data']['id'] = $candidate_id;

			// LOG
			qlog($_SESSION['user']['id'], 'candidato eliminado', 'candidates', "{$candidate_id}");

			candidate_get();

			response_send(true, 'candidato eliminado con éxito');
    } else {
			response_send(false, 'candidato eliminado sin éxito');
    }
	}

?>
