var teamsImage = document.getElementById("teamsImage");

var imageArray = ["/Content/WALL.jpg", "/Content/rog5-550.jpg"];

var imageIndex = 0;

function changeImage() {
    teamsImage.setAttribute("src", imageArray[imageIndex])
    imageIndex++;
    if (imageIndex >= imageArray.length) {
        imageIndex = 0;
    }
}

setInterval(changeImage, 5000);