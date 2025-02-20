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
    }
});
