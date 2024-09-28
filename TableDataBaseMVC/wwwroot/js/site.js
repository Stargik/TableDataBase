function openHtmlPreview(htmlContent) {
    var newWindow = window.open("", "_blank", "width=600,height=400");
    newWindow.document.open();
    newWindow.document.write(htmlContent);
    newWindow.document.close();
}