/* This App was created By Mateo Saldivar to edit levels for the game created for Tagwizz
 *  Version 1.0.1
 */


//App Variables
String fileExportPath = "../../Assets/Levels/LevelData.txt";
ArrayList<int[]> levels = new ArrayList<int[]>();
int CurrentLevel = 0;
int LayoutType = 1;
int GridStartX = 80;
int GridsStartY = 110;
int CellSize = 90;
int fontSize = 20;
//Button initializer
Button mode_1 = new Button(0, 0, 300, 50);
Button mode_2 = new Button(300, 0, 300, 50);
Button back = new Button(0, 0, 100, 30);
Button next = new Button(0, 0, 100, 30);
Button erase = new Button(0, 0, 100, 30);
Button save = new Button(0, 0, 100, 30);

void setup() {
  size(600, 820);
  textSize(fontSize);
  LoadLevels();
}

void draw() {
  background(255);
  DrawButtons();
  DrawGrid();
}

void mousePressed() {
  GridPressed();
  ButtonPressed();
}

void LoadLevels() {
  if (!LoadFile()) {
    levels.add(new int[35]);
    for (int i = 0; i < levels.get(CurrentLevel).length; i++) {
      levels.get(CurrentLevel)[i]=0;
    }
  }
}

boolean LoadFile() {
  File f = dataFile("../"+fileExportPath);
  if (f.isFile()) {
    String[] input = loadStrings(fileExportPath);
    if (input.length > 0) {
      for (int i = 0; i < input.length; i++) {
        String[] levelData = input[i].split(",");
        int[] levelValues = new int[35];
        for (int j = 0; j < levelData.length; j++) {
          levelValues[j] = int(levelData[j]);
        }
        if (!EmptyLevel(levelValues))
          levels.add(levelValues);
      }
      if (levels.size() > 0)
        return true;
    }
  }
  return false;
}

void DrawButtons() {
  mode_1.render(0, 0, "Mode 1");
  mode_2.render(300, 0, "Mode 2");

  if (CurrentLevel > 0)back.render(10, 760, "previous");
  next.render(width-110, 760, "next");

  erase.render(width/2-50, 760, "erase");
  save.render(width/2-50, 65, "save");

  text("Level: "+(CurrentLevel+1), width/2-30, 810);
}

void DrawGrid() {
  float OffsetX = 0;
  GridStartX = LayoutType == 1 ? 80 : 50;
  for (int i = 0; i < 35; i++) {
    if (levels.get(CurrentLevel)[i]==1) fill(color(#16C100));
    if (levels.get(CurrentLevel)[i]==2) fill(color(#3533F0));
    if (levels.get(CurrentLevel)[i]==3) fill(color(#F03336));
    if (levels.get(CurrentLevel)[i]==0) fill(255);
    if (i%5==0 && LayoutType == 2) {
      if (OffsetX ==0 )OffsetX+=CellSize/2;
      else OffsetX=0;
    }
    rect(GridStartX+(i % 5)*CellSize + OffsetX, GridsStartY+((int)i/5)*CellSize, CellSize, CellSize);
  }
  fill(255);
}

void GridPressed() {
  float OffsetX = 0;
  for (int i = 0; i < 35; i++) {
    if (i%5==0 && LayoutType == 2) {
      if (OffsetX == 0 )OffsetX+=CellSize/2;
      else OffsetX=0;
    }
    if (GridButton((GridStartX+(i % 5)*CellSize) + OffsetX, GridsStartY+((int)i/5)*CellSize, CellSize, CellSize)) {
      if (mouseButton == LEFT) {
        levels.get(CurrentLevel)[i]++;
      } else if (mouseButton == RIGHT) {
        levels.get(CurrentLevel)[i]--;
      }

      if (levels.get(CurrentLevel)[i] > 3) levels.get(CurrentLevel)[i] = 0;
      if (levels.get(CurrentLevel)[i] < 0) levels.get(CurrentLevel)[i] = 3;
    }
  }
}

void ButtonPressed() {
  if (mode_1.Pressed()) LayoutType = 1;
  if (mode_2.Pressed()) LayoutType = 2;
  if (erase.Pressed()) EraseGrid();
  if (next.Pressed()) NextLevel();
  if (back.Pressed() && CurrentLevel > 0)CurrentLevel--;
  if (save.Pressed()) SaveFile();
}

void NextLevel() {
  CurrentLevel++;
  if (CurrentLevel > levels.size()-1) {
    levels.add(new int[35]);
    for (int i = 0; i < levels.get(CurrentLevel).length; i++) {
      levels.get(CurrentLevel)[i]=0;
    }
  }
}

void EraseGrid() {
  for (int i = 0; i < levels.get(CurrentLevel).length; i++) {
    levels.get(CurrentLevel)[i]=0;
  }
}

void SaveFile() {
  String[] OutPut = new String[levels.size()];
  int outPutIndex = 0;
  int ResizeArrayValue = 0;
  for (int i = 0; i < levels.size(); i++) {
    if (!EmptyLevel(levels.get(i))) {
      OutPut[outPutIndex] = "";
      for (int j = 0; j < levels.get(i).length; j++) {
        OutPut[outPutIndex]+=levels.get(i)[j] + (j >= levels.get(i).length-1 ? "" : ",") ;
      }
      outPutIndex++;
    } else {
      ResizeArrayValue++;
    }
  }
  String[] ExportStrings = new String[OutPut.length-ResizeArrayValue];
  for (int i = 0; i < ExportStrings.length; i++) {
    ExportStrings[i] = OutPut[i];
  }
  saveStrings(fileExportPath, ExportStrings);
}

boolean EmptyLevel(int[] lvl) {
  for (int i : lvl)
    if (i!=0)return false;

  return true;
}


class Button {
  float m_x, m_y, m_w, m_h;
  String m_text  = "";
  color m_hover_color = color(200);
  Button(float x, float y, float w, float h) {
    m_x=x;
    m_y=y;
    m_w=w;
    m_h=h;
  }

  void render(float x, float y, String txt) {
    m_text = txt;
    m_x = x;
    m_y = y;
    fill(Pressed() ? m_hover_color : 255);
    rect(m_x, m_y, m_w, m_h);
    fill(0);
    text(m_text, (m_x+m_w/2)-(m_text.length()*fontSize)/4, (m_y+m_h/2)+fontSize/4);
  }

  boolean Pressed() {
    return mouseX >= m_x && mouseX < m_x+m_w && mouseY >= m_y && mouseY < m_y+m_h;
  }
};

boolean GridButton(float x, float y, float w, float h) {
  return mouseX >= x && mouseX < x+w && mouseY >= y && mouseY < y+h;
}
