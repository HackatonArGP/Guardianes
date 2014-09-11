<?php
require_once('lib/nusoap.php');

$id_ciberaccion = intval($_REQUEST['id_ciberaccion']);
$mail = utf8_encode($_REQUEST['mail']);

//$c = new nusoap_client('https://localhost:81/ws2/userserver.php');
$c = new nusoap_client('https://ciberacciones.greenpeace.org.ar/cyberacciones/ws/userserver.php');

$response = $c->call('getUser', array('mail' => $mail,'id_ciberaccion' => $id_ciberaccion));
							
echo json_encode($response);
//print_r($response);

?>