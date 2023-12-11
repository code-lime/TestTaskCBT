/**
 * Returns a JS object representation of a Javascript Web Token from its common encoded
 * string form.
 *
 * @param {string} token a Javascript Web Token in base64 encoded, `.` separated form
 * @returns {(object | undefined)} an object-representation of the token
 * or undefined if parsing failed
 */
function getParsedJwt(token) {
    try {
        return JSON.parse(atob(token.split('.')[1]))
    } catch (error) {
        return undefined
    }
}
/**
 * @returns {(object | undefined)}
 */
function getParsedCurrentJwt() {
    let accessToken = localStorage.getItem('accessToken');
    return accessToken !== null && accessToken !== undefined
        ? getParsedJwt(accessToken)
        : undefined;
}
function initFetchJwt(refreshApi) {
    const originalFetch = fetch;
    fetch = function () {
        let self = this;
        let args = arguments;
        let init = args.length >= 2 ? args[1] : null;
        if (init !== null && init !== undefined && 'jwt' in init) {
            let jwt = init['jwt'];
            let accessToken;
            let refreshToken;
            if (jwt === 'local') {
                accessToken = localStorage.getItem('accessToken');
                refreshToken = localStorage.getItem('refreshToken');
            } else {
                accessToken = jwt['accessToken'];
                refreshToken = jwt['refreshToken'];
            }
            let headers;
            if ('headers' in init) headers = init.headers;
            else init.headers = headers = {};

            headers['Authorization'] = 'Bearer ' + accessToken;

            return originalFetch.apply(self, args).then(async function (data) {
                if (data.status !== 401) return data;
                let response = await originalFetch(`${refreshApi}?refreshToken=${refreshToken}`);
                if (response.status !== 200) return response;
                let json = await response.json();
                if (json['status'] !== 'success') return response;
                let auth = json['response'];
                accessToken = auth['accessToken'];
                refreshToken = auth['refreshToken'];
                if (jwt === 'local') {
                    localStorage.setItem('accessToken', accessToken);
                    localStorage.setItem('refreshToken', refreshToken);
                }
                headers['Authorization'] = 'Bearer ' + accessToken;
                response = await originalFetch(...args);
                return response;
            });
        }
        return originalFetch(...args);
    };
}