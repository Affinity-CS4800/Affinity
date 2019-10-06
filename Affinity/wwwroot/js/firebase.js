var firebaseConfig = {
    apiKey: FIREBASE_CONFIG.apiKey,
    authDomain: FIREBASE_CONFIG.authDomain,
    databaseURL: FIREBASE_CONFIG.databaseURL,
    projectId: FIREBASE_CONFIG.projectId,
    storageBucket: "",
    messagingSenderId: FIREBASE_CONFIG.messagingSenderId,
    appId: FIREBASE_CONFIG.appId,
    measurementId: FIREBASE_CONFIG.measurementId
};

firebase.initializeApp(firebaseConfig);

