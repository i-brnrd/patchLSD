mergeInto(LibraryManager.library, {
    DownloadFile: function (fileNamePtr, fileContentPtr) {
        // 1. Convert the C# string pointers to JS strings
        var fileName = UTF8ToString(fileNamePtr);
        var fileContent = UTF8ToString(fileContentPtr);

        // 2. Create a Blob containing the text
        var blob = new Blob([fileContent], { type: 'text/plain' });

        // 3. Create a temporary <a> link to trigger download
        var link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = fileName;

        // 4. Append link, click, remove
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    },
 DownloadZippedData: function(fileNamePtr, base64DataPtr) {
        // 1. Convert pointers to JS strings
        var fileName = UTF8ToString(fileNamePtr);
        var base64Data = UTF8ToString(base64DataPtr);

        // 2. Convert base64 -> raw bytes (Uint8Array)
        var byteCharacters = atob(base64Data);
        var byteNumbers = new Array(byteCharacters.length);
        for (var i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        var byteArray = new Uint8Array(byteNumbers);

        // 3. Create a blob from those bytes
        var blob = new Blob([byteArray], { type: 'application/zip' });

        // 4. Trigger the download
        var link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = fileName; // e.g. "myData.zip"
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }

});
