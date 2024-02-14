<?php 
$images  = glob("*.jpg");



function fn_resize($image_resource_id,$width,$height) {
$target_width =200;
$target_height =200;
$target_layer=imagecreatetruecolor($target_width,$target_height);
imagecopyresampled($target_layer,$image_resource_id,0,0,0,0,$target_width,$target_height, $width,$height);
return $target_layer;
}

foreach ($images as  $value) {

	$source_properties = getimagesize($value);
	$image_type = $source_properties[2]; 
	
	$image_resource_id = imagecreatefromjpeg($value);  
	$target_layer = fn_resize($image_resource_id,$source_properties[0],$source_properties[1]);
	imagejpeg($target_layer,$_FILES['myImage']['name'] . $value);
}







 ?>