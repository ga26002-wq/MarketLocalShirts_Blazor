window.descargarArchivo = (nombre, base64, tipo) => {
    const binario = atob(base64);
    const bytes = new Uint8Array(binario.length);
    for (let i = 0; i < binario.length; i++)
        bytes[i] = binario.charCodeAt(i);

    const blob = new Blob([bytes], { type: tipo });
    const url = URL.createObjectURL(blob);
    const enlace = document.createElement("a");
    enlace.href = url;
    enlace.download = nombre;
    document.body.appendChild(enlace);
    enlace.click();
    enlace.remove();
    URL.revokeObjectURL(url);
};
