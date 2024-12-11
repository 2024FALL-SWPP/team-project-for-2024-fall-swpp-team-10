mergeInto(LibraryManager.library, {
    DownloadScreenshot: function (fileNamePtr, imageDataPtr, imageDataLength) {
        // Unity에서 전달된 데이터를 읽음
        var fileName = UTF8ToString(fileNamePtr);
        var imageData = new Uint8Array(Module.HEAPU8.buffer, imageDataPtr, imageDataLength);

        // Blob 생성
        var blob = new Blob([imageData], { type: "image/jpeg" });

        // 브라우저에서 파일 다운로드
        var link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
});
