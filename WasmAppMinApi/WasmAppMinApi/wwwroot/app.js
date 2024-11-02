function setAntiforgeryCookie(key, value) {
    let now = new Date();
    document.cookie = `${key}=${value}`;
}
