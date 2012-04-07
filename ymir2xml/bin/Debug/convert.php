<?php

mysql_connect("192.168.56.102","root","daroo");

$xml = simplexml_load_file("group.xml");
$groups = array();

foreach($xml->groups->group as $gkey => $group)
{
	$mobs = array();
	foreach($group->mob as $mkey => $mob)
	{
		$vnum = (string) $mob[0];
		$sql = "SELECT locale_name FROM player.mob_proto WHERE vnum = '".$vnum."'";
		$q = mysql_query($sql) or die(mysql_error());
		$data = mysql_fetch_object($q);
		$mobs[] = array($vnum, @$data->locale_name);
		if(mysql_num_rows($q) == 1)
		{
			$mob->attributes()->name = $data->locale_name;
		}
		else
		{
			//unset($xml->groups->group[$gkey]->mob[$mkey]);
		}
	}
	
	$groups[ (string)$group["vnum"] ] = $mobs;
}

print_r($xml);

?>