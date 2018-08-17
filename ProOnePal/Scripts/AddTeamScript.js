function selectedImage () {
    var teamSelected = document.getElementById("teamId").value;
    ProOnePal.OnePalService.getPathfromName(teamSelected, changeImage);   
}

function changeImage(result) {
    var image = document.getElementById("teamsImage");
    image.setAttribute("src", result);
}