/**
 * @param {string} api
*/
async function preloadUser(api) {
    initFetchJwt(`${api}/auth/refresh`, () => window.location = 'auth');
    let jwt = getParsedCurrentJwt();
    if (jwt === undefined) {
        window.location = 'auth';
        return undefined;
    }
    let response = await fetch(`${api}/users/0`, { jwt: 'local' });
    if (response.status !== 200) {
        window.location = 'auth';
        return undefined;
    }
    let json = await response.json();
    if (json.status !== 'success' || !('response' in json)) {
        window.location = 'auth';
        return undefined;
    }
    return json.response;
}
/**
 * @param {{tag:string, options?:ElementCreationOptions, child?:tree, childs?:Array<tree>, setup?:function(HTMLElement)}} tree
*/
function createTree(tree) {
    let current = document.createElement(tree.tag, tree.options);
    if ('child' in tree) {
        current.appendChild(createTree(tree.child));
    }
    if ('childs' in tree) {
        for (let child of tree.childs)
            current.appendChild(createTree(child));
    }
    if ('setup' in tree) {
        tree.setup(current);
    }
    return current;
}

function deepClone(obj) {
    return JSON.parse(JSON.stringify(obj));
}

/**
 * @param {HTMLElement} table
 * @param {{key: string}} params
 * @param {function({key: string}):void} changeCallback
*/
function createParamsTable(table, params, changeCallback) {
    params = deepClone(params);
    /**@type {Array<string>}*/
    let paramsEntries = Object.entries(params);

    table.innerHTML = '';

    for (let [paramName, paramValue] of paramsEntries) {
        let optional = paramName.startsWith('?');
        if (optional) paramName = paramName.substring(1);
        /**@type {HTMLElement}*/
        let valueInput;
        table.appendChild(createTree({
            tag: 'tr',
            childs: [
                {
                    tag: 'td',
                    setup: function (element) {
                        element.textContent = paramName + (optional ? '' : '*');
                    }
                },
                {
                    tag: 'td',
                    child: {
                        tag: 'input',
                        setup: function (element) {
                            valueInput = element;
                            element.setAttribute('type', 'text');
                            element.setAttribute('value', paramValue);
                            element.style.height = '100%';
                            element.style.width = '100%';
                        }
                    }
                },
                {
                    tag: 'td',
                    child: {
                        tag: 'input',
                        setup: function (element) {
                            element.setAttribute('type', 'button');
                            element.setAttribute('value', '$');
                            element.style.backgroundColor = '#00ff00';
                            element.style.height = '30px';
                            element.style.width = '30px';

                            element.onclick = function () {
                                /**@type {string}*/
                                let valueText = valueInput.value;
                                valueText = valueText.trim();
                                if (!optional && valueText.length === 0) {
                                    alert('Ошибка изменения! Значение поля обязательно');
                                    return;
                                }
                                params[(optional ? '?' : '') + paramName] = valueText;
                                changeCallback(params);
                            }
                        }
                    }
                }
            ]
        }));
    }
}
/**
 * @param {HTMLElement} table
 * @param {{key: string}} fields
 * @param {function({key: string}):void} changeCallback
*/
function createFieldsTable(table, fields, changeCallback) {
    params = deepClone(params);
    /**@type {Array<string>}*/
    let fieldEntries = Object.entries(fields);

    table.innerHTML = '';

    for (let [fieldName, fieldValue] of fieldEntries) {
        table.appendChild(createTree({
            tag: 'tr',
            childs: [
                {
                    tag: 'td',
                    setup: function (element) {
                        element.textContent = fieldName;
                    }
                },
                {
                    tag: 'td',
                    setup: function (element) {
                        element.textContent = fieldValue;
                    }
                },
                {
                    tag: 'td',
                    child: {
                        tag: 'input',
                        setup: function (element) {
                            element.setAttribute('type', 'button');
                            element.setAttribute('value', '-');
                            element.style.backgroundColor = '#ff0000';
                            element.style.height = '30px';
                            element.style.width = '30px';
                            element.onclick = function () {
                                delete fields[fieldName];
                                changeCallback(fields);
                            }
                        }
                    }
                }
            ]
        }));
    }

    /**@type {HTMLElement}*/
    var keyInput;

    /**@type {HTMLElement}*/
    var valueInput;

    table.appendChild(createTree({
        tag: 'tr',
        childs: [
            {
                tag: 'td',
                child: {
                    tag: 'input',
                    setup: function (element) {
                        keyInput = element;
                        element.setAttribute('type', 'text');
                        element.setAttribute('value', '');
                        element.style.height = '100%';
                        element.style.width = '100%';
                    }
                }
            },
            {
                tag: 'td',
                child: {
                    tag: 'input',
                    setup: function (element) {
                        valueInput = element;
                        element.setAttribute('type', 'text');
                        element.setAttribute('value', '');
                        element.style.height = '100%';
                        element.style.width = '100%';
                    }
                }
            },
            {
                tag: 'td',
                child: {
                    tag: 'input',
                    setup: function (element) {
                        element.setAttribute('type', 'button');
                        element.setAttribute('value', '+');
                        element.style.backgroundColor = '#00ff00';
                        element.style.height = '30px';
                        element.style.width = '30px';
                        element.onclick = function () {
                            /**@type {string}*/
                            let keyText = keyInput.value;
                            keyText = keyText.trim();
                            /**@type {string}*/
                            let valueText = valueInput.value;
                            valueText = valueText.trim();
                            if (keyText.length === 0 || valueText.length === 0) {
                                alert('Ошибка добавления! Название или значение поля пустое');
                                return;
                            }
                            if (keyText in fields) {
                                alert('Ошибка добавления! Название поля совпадает с существующим полем');
                                return;
                            }
                            fields[keyText] = valueText;
                            changeCallback(fields);
                        }
                    }
                }
            }
        ]
    }));
}