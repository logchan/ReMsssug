#!/usr/local/bin/php
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Re:MSSSUG iHome Login</title>
</head>
<body>
<?php 
    $f = fopen("server", "r") or die("server error 0");
    $server = fread($f, filesize("server"));
    fclose($f);
    
    $f = fopen("psk", "r") or die("server error 1");
    $psk = fread($f, filesize("psk"));
    fclose($f);
    
    $itsc = $_SERVER["REMOTE_USER"];
    echo "Logging you in as ".$itsc.", please wait...";
    
    $time = gmdate("Y-m-d H:i:s");
    $hash = md5($itsc.$time.$psk);
    $submit = $server."/processlogin";
?>
<div style="display: none">
<form action="<?php echo $submit ?>" method="post" id="loginform">
<input type="hidden" name="time" value="<?php echo $time?>" />
<input type="hidden" name="itsc" value="<?php echo $itsc?>" />
<input type="hidden" name="hash" value="<?php echo $hash?>" />
</form>
<script>
setTimeout(function() { document.getElementById('loginform').submit(); }, 500);
</script>
</div>
</body>
</html>