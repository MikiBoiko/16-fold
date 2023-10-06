import { IonContent, IonHeader, IonPage, IonTitle, IonToolbar } from '@ionic/react';
import './Home.css';
import TimerTest from '../components/TimerTest';

const Home: React.FC = () => {
  return (
    <IonPage>
      <IonHeader>
        <IonToolbar>
          <IonTitle>Timer test</IonTitle>
        </IonToolbar>
      </IonHeader>
      <IonContent fullscreen>
        <IonHeader collapse="condense">
          <IonToolbar>
            <IonTitle size="large">Timer test</IonTitle>
          </IonToolbar>
        </IonHeader>
        <TimerTest />
      </IonContent>
    </IonPage>
  );
};

export default Home;
